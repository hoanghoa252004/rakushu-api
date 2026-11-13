using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AudioClips;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GenerateShadowingMaterials;

public sealed record GenerateShadowingMaterialsCommand(int BatchSize = 5) : IRequest<Result<int>>;

internal sealed class GenerateShadowingMaterialsHandler(
	IShadowingMaterialRepository materialRepository,
	IShadowingRoleRepository roleRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IMediaConvertService audioClipService,
	IUnitOfWork unitOfWork,
	ILogger<GenerateShadowingMaterialsHandler> logger)
	: IRequestHandler<GenerateShadowingMaterialsCommand, Result<int>>
{
	private sealed record Group(SpeakerId SpeakerId, string Text, TimeSpan Start, TimeSpan End, List<TranscriptSegmentId> SegmentIds);

	public async Task<Result<int>> Handle(GenerateShadowingMaterialsCommand request, CancellationToken cancellationToken)
	{
		var materials = await materialRepository.GetProcessingAsync(request.BatchSize, cancellationToken);
		var processed = 0;

		foreach (var material in materials)
		{
			try
			{
				var result = await ProcessAsync(material, cancellationToken);
				if (result.IsSuccess)
					processed++;
				else
					logger.LogWarning("Shadowing material {Id} generation failed: {Error}", material.Id.Value, result.Error);
			}
			catch (Exception ex) when (ex is not OperationCanceledException)
			{
				logger.LogError(ex, "Shadowing material {Id} generation threw.", material.Id.Value);
			}
		}

		return Result.Success(processed);
	}

	private async Task<Result> ProcessAsync(ShadowingMaterial material, CancellationToken ct)
	{
		var start = await materialRepository.GetVideoSegmentAsync(material.VideoId, material.StartTranscriptSegmentId, ct);
		var end = await materialRepository.GetVideoSegmentAsync(material.VideoId, material.EndTranscriptSegmentId, ct);
		if (start is null || end is null)
			return Result.Failure(ShadowingMaterialErrors.SegmentNotFound);

		var existingRoles = await roleRepository.GetByMaterialIdAsync(material.Id, ct);
		if (existingRoles.Count == 0)
			return Result.Failure(ShadowingMaterialErrors.NoTranscriptSpeakers);
		var roles = existingRoles.ToDictionary(r => r.SpeakerId, r => r);

		var segments = await materialRepository.GetSegmentsBetweenAsync(material.VideoId, start.Sequence, end.Sequence, ct);

		var groups = new List<Group>();
		Group? current = null;
		foreach (var s in segments)
		{
			var speakerId = s.TranscriptSpeaker?.SpeakerId;
			if (speakerId is null || !roles.ContainsKey(speakerId))
			{
				current = null;
				continue;
			}

			if (current is not null && current.SpeakerId == speakerId)
			{
				current.SegmentIds.Add(s.Id);
				var merged = current with { Text = current.Text + s.Text, End = s.EndTime };
				groups[^1] = merged;
				current = merged;
			}
			else
			{
				current = new Group(speakerId, s.Text, s.StartTime, s.EndTime, [s.Id]);
				groups.Add(current);
			}
		}

		if (groups.Count == 0)
			return Result.Failure(ShadowingMaterialErrors.NoTranscriptSpeakers);

		var sourceKey = await materialRepository.GetVideoSourceKeyAsync(material.VideoId, ct);
		if (string.IsNullOrWhiteSpace(sourceKey))
			return Result.Failure(ShadowingMaterialErrors.VideoNotReady);

		var audioKeys = await Task.WhenAll(groups.Select((g, i) =>
			audioClipService.ExtractClipAsync(
				sourceKey,
				$"shadowing/{material.Id.Value}/lines/{i + 1}",
				g.Start,
				g.End,
				ct)));

		return await unitOfWork.ExecuteAsync(async () =>
		{
			for (var i = 0; i < groups.Count; i++)
			{
				var g = groups[i];
				var lineResult = ShadowingRoleLine.Create(
					roles[g.SpeakerId].Id, i + 1, g.Text, g.Start, g.End, audioKeys[i]);
				if (lineResult.IsFailure)
					return Result.Failure(lineResult.Error);

				lineResult.Value.SetSegments(g.SegmentIds);
				roleLineRepository.Add(lineResult.Value);
			}

			var statusResult = material.ChangeStatus(ShadowingMaterialStatus.Ready);
			if (statusResult.IsFailure)
				return Result.Failure(statusResult.Error);

			return Result.Success();
		}, ct);
	}
}
