using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

public sealed class ShadowingRoleLineTranscriptSegment
{
	public ShadowingRoleLineId ShadowingRoleLineId { get; private set; } = null!;
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;

	// Denormalized to enforce "a segment appears once per material" with a unique index.
	public ShadowingMaterialId ShadowingMaterialId { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	public ShadowingRoleLine ShadowingRoleLine { get; private set; } = null!;
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;

	private ShadowingRoleLineTranscriptSegment() { }

	internal ShadowingRoleLineTranscriptSegment(
		ShadowingRoleLineId shadowingRoleLineId,
		TranscriptSegmentId transcriptSegmentId,
		ShadowingMaterialId shadowingMaterialId)
	{
		ShadowingRoleLineId = shadowingRoleLineId;
		TranscriptSegmentId = transcriptSegmentId;
		ShadowingMaterialId = shadowingMaterialId;
	}
}
