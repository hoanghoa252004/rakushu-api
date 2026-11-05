using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AudioClips;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
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
	IAudioClipService audioClipService,
	IUnitOfWork unitOfWork,
	ILogger<GenerateShadowingMaterialsHandler> logger)
	: IRequestHandler<GenerateShadowingMaterialsCommand, Result<int>>
{
	private sealed record Group(SpeakerId SpeakerId, string Text, TimeSpan Start, TimeSpan End);

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
		var segments = await materialRepository.GetTranscriptSegmentsAsync(material.VideoId, ct);

		var groups = new List<Group>();
		foreach (var s in segments)
		{
			var speakerId = s.TranscriptSpeaker?.SpeakerId;
			if (speakerId is null)
				continue;

			var last = groups.Count > 0 ? groups[^1] : null;
			if (last is not null && last.SpeakerId == speakerId)
				groups[^1] = last with { Text = last.Text + s.Text, End = s.EndTime };
			else
				groups.Add(new Group(speakerId, s.Text, s.StartTime, s.EndTime));
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
			var roles = new Dictionary<SpeakerId, ShadowingRole>();
			foreach (var g in groups)
			{
				if (roles.ContainsKey(g.SpeakerId))
					continue;

				var roleResult = ShadowingRole.Create(material.Id, g.SpeakerId, roles.Count + 1);
				if (roleResult.IsFailure)
					return Result.Failure(roleResult.Error);

				roleRepository.Add(roleResult.Value);
				roles[g.SpeakerId] = roleResult.Value;
			}

			for (var i = 0; i < groups.Count; i++)
			{
				var g = groups[i];
				var lineResult = ShadowingRoleLine.Create(
					material.Id, roles[g.SpeakerId].Id, i + 1, g.Text, g.Start, g.End, audioKeys[i]);
				if (lineResult.IsFailure)
					return Result.Failure(lineResult.Error);

				roleLineRepository.Add(lineResult.Value);
			}

			var statusResult = material.ChangeStatus(ShadowingMaterialStatus.Reviewing);
			if (statusResult.IsFailure)
				return Result.Failure(statusResult.Error);

			return Result.Success();
		}, ct);
	}
}
