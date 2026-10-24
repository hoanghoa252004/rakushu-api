namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Filter options for querying SRS items of a user.
/// </summary>
public enum SrsItemFilter
{
	/// <summary>
	/// Active learning items (Learning, Review, New; excludes Suspended and Ignored).
	/// </summary>
	Active = 0,

	/// <summary>
	/// Suspended items only.
	/// </summary>
	Suspended = 1,

	/// <summary>
	/// Items currently in the initial learning phase.
	/// </summary>
	Learning = 2,

	/// <summary>
	/// Items currently in the periodic review phase.
	/// </summary>
	Review = 3,

	/// <summary>
	/// Soft-deleted or ignored items.
	/// </summary>
	Ignored = 4
}
