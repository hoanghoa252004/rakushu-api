using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

public sealed class ShadowingRoleLine : Entity<ShadowingRoleLineId>
{
	public ShadowingMaterialId ShadowingMaterialId { get; private set; } = null!;
	public ShadowingRoleId ShadowingRoleId { get; private set; } = null!;
	public int SortOrder { get; private set; }
	public string Text { get; private set; } = null!;
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public string AudioKey { get; private set; } = null!;

	// COMPUTED PROPERTIES
	public TimeSpan Duration => EndTime - StartTime;


	// NAVIGATION PROPERTIES
	// ShadowingMaterial
	public ShadowingMaterial.ShadowingMaterial ShadowingMaterial { get; private set; } = null!;

	// ShadowingRole
	public ShadowingRole.ShadowingRole ShadowingRole { get; private set; } = null!;

	// ShadowingRoleLineTranscriptSegments
	//private readonly List<ShadowingRoleLineTranscriptSegment>
	//	_transcriptSegments = new();

	//public IReadOnlyCollection<ShadowingRoleLineTranscriptSegment>
	//	TranscriptSegments => _transcriptSegments.AsReadOnly();

	private ShadowingRoleLine() { }

}
