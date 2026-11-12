using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials;

internal static class ShadowingMaterialRangeValidator
{
	// Validates the segment range and selected transcript speakers; returns the distinct speakers (ordered by first appearance).
	public static async Task<Result<IReadOnlyList<SpeakerId>>> ValidateAsync(
		IShadowingMaterialRepository repository,
		VideoId videoId,
		Guid startSegmentId,
		Guid endSegmentId,
		IReadOnlyCollection<Guid> speakerIds,
		ShadowingMaterialId? excludeMaterialId,
		CancellationToken ct)
	{
		var start = await repository.GetVideoSegmentAsync(videoId, TranscriptSegmentId.From(startSegmentId), ct);
		var end = await repository.GetVideoSegmentAsync(videoId, TranscriptSegmentId.From(endSegmentId), ct);
		if (start is null || end is null)
			return Result.Failure<IReadOnlyList<SpeakerId>>(ShadowingMaterialErrors.SegmentNotFound);

		if (start.Sequence > end.Sequence)
			return Result.Failure<IReadOnlyList<SpeakerId>>(ShadowingMaterialErrors.InvalidSegmentRange);

		if (await repository.RangeExistsAsync(start.Id, end.Id, excludeMaterialId, ct))
			return Result.Failure<IReadOnlyList<SpeakerId>>(ShadowingMaterialErrors.RangeAlreadyExists);

		var requested = speakerIds.Distinct().ToHashSet();
		var segments = await repository.GetSegmentsBetweenAsync(videoId, start.Sequence, end.Sequence, ct);

		var ordered = new List<SpeakerId>();
		foreach (var seg in segments)
		{
			var sid = seg.TranscriptSpeaker?.SpeakerId;
			if (sid is not null && requested.Contains(sid.Value) && !ordered.Contains(sid))
				ordered.Add(sid);
		}

		if (ordered.Count != requested.Count)
			return Result.Failure<IReadOnlyList<SpeakerId>>(ShadowingMaterialErrors.TranscriptSpeakerInvalid);

		return Result.Success<IReadOnlyList<SpeakerId>>(ordered);
	}
}
