using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines;

internal sealed record ResolvedLineSegments(
	IReadOnlyList<TranscriptSegmentId> Ids,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime);

internal static class RoleLineSegmentResolver
{
	public static async Task<Result<ResolvedLineSegments>> ResolveAsync(
		IShadowingRoleLineRepository repository,
		ShadowingMaterial material,
		IEnumerable<Guid> segmentIds,
		ShadowingRoleLineId? excludeLineId,
		CancellationToken cancellationToken)
	{
		var ids = segmentIds.Distinct().Select(TranscriptSegmentId.From).ToList();
		if (ids.Count == 0)
			return Result.Failure<ResolvedLineSegments>(ShadowingRoleLineErrors.TranscriptSegmentsRequired);

		var segments = await repository.GetVideoSegmentsAsync(material.VideoId, ids, cancellationToken);
		if (segments.Count != ids.Count)
			return Result.Failure<ResolvedLineSegments>(ShadowingRoleLineErrors.TranscriptSegmentNotFound);

		if (await repository.AnySegmentUsedAsync(material.Id, ids, excludeLineId, cancellationToken))
			return Result.Failure<ResolvedLineSegments>(ShadowingRoleLineErrors.TranscriptSegmentAlreadyUsed);

		var ordered = segments.OrderBy(s => s.Sequence).ToList();
		for (var i = 1; i < ordered.Count; i++)
		{
			if (ordered[i].Sequence != ordered[i - 1].Sequence + 1)
				return Result.Failure<ResolvedLineSegments>(ShadowingRoleLineErrors.TranscriptSegmentsNotContiguous);
		}

		return Result.Success(new ResolvedLineSegments(
			ordered.Select(s => s.Id).ToList(),
			string.Concat(ordered.Select(s => s.Text)),
			ordered.Min(s => s.StartTime),
			ordered.Max(s => s.EndTime)));
	}
}
