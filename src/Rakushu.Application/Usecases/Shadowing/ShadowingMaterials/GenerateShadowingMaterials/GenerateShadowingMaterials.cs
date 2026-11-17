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

		var lines = await roleLineRepository.GetByMaterialIdAsync(material.Id, ct);
		if (lines.Count == 0)
		{
			var createdLines = await CreateLinesAsync(material, start, end, roles, ct);
			if (createdLines.IsFailure)
				return Result.Failure(createdLines.Error);
			lines = createdLines.Value;
		}

		return await DispatchClipJobsAsync(material, lines, ct);
	}

	private async Task<Result<IReadOnlyList<ShadowingRoleLine>>> CreateLinesAsync(
		ShadowingMaterial material,
		TranscriptSegment start,
		TranscriptSegment end,
		Dictionary<SpeakerId, ShadowingRole> roles,
		CancellationToken ct)
	{
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
			return Result.Failure<IReadOnlyList<ShadowingRoleLine>>(ShadowingMaterialErrors.NoTranscriptSpeakers);

		var created = new List<ShadowingRoleLine>();
		var saved = await unitOfWork.ExecuteAsync(async () =>
		{
			for (var i = 0; i < groups.Count; i++)
			{
				var g = groups[i];
				// The role line id is known only after creation, so the clip key is assigned once the line exists.
				var lineResult = ShadowingRoleLine.Create(
					roles[g.SpeakerId].Id, i + 1, g.Text, g.Start, g.End, "pending");
				if (lineResult.IsFailure)
					return Result.Failure(lineResult.Error);

				var line = lineResult.Value;
				line.SetAudioKey(ClipOutputKey(material, line) + ".m4a");
				line.SetSegments(g.SegmentIds);
				roleLineRepository.Add(line);
				created.Add(line);
			}

			return Result.Success();
		}, ct);

		if (saved.IsFailure)
			return Result.Failure<IReadOnlyList<ShadowingRoleLine>>(saved.Error);
		return Result.Success<IReadOnlyList<ShadowingRoleLine>>(created);
	}

	private static string ClipOutputKey(ShadowingMaterial material, ShadowingRoleLine line)
		=> $"shadowing/{material.Id.Value}/lines/{line.Id.Value}";

	private async Task<Result> DispatchClipJobsAsync(
		ShadowingMaterial material,
		IReadOnlyList<ShadowingRoleLine> lines,
		CancellationToken ct)
	{
		var sourceKey = await materialRepository.GetVideoSourceKeyAsync(material.VideoId, ct);
		if (string.IsNullOrWhiteSpace(sourceKey))
			return Result.Failure(ShadowingMaterialErrors.VideoNotReady);

		var jobs = await audioClipService.ListClipJobsAsync(lines.Select(l => l.Id.Value).ToList(), ct);
		var pending = 0;

		foreach (var line in lines)
		{
			var lineJobs = jobs.Where(j => j.RoleLineId == line.Id.Value).ToList();
			if (lineJobs.Any(j => j.Status == NormalizeJobStatus.Complete))
				continue;

			pending++;
			if (lineJobs.Any(j => j.Status == NormalizeJobStatus.InProgress))
				continue;

			if (lineJobs.Count > 0)
				logger.LogWarning("Clip job failed for role line {LineId}: {Error}. Retrying.", line.Id.Value, lineJobs[0].Error);

			await audioClipService.CreateClipJobAsync(
				line.Id.Value, sourceKey, ClipOutputKey(material, line), line.StartTime, line.EndTime, ct);
		}

		if (pending > 0)
			return Result.Success();

		return await unitOfWork.ExecuteAsync(async () =>
		{
			var statusResult = material.ChangeStatus(ShadowingMaterialStatus.Ready);
			return statusResult.IsFailure ? Result.Failure(statusResult.Error) : Result.Success();
		}, ct);
	}
}
