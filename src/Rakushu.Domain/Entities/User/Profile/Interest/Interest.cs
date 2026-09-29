using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.ContentCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Profile.Interest;

public sealed class Interest : Entity<InterestId>
{
	public ProfileId ProfileId { get; private set; } = null!;
	public ContentCategoryId ContentCategoryId { get; private set; } = null!;
	public int Priority { get; private set; }

	// NAVIGATION PROPERTIES
	// Profile
	public Profile Profile { get; private set; } = null!;

	// ContentCategory
	public ContentCategory.ContentCategory ContentCategory { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS----------
	private Interest() { }

	private Interest(
		InterestId id,
		ProfileId profileId,
		ContentCategoryId contentCategoryId,
		int priority) : base(id)
	{
		ProfileId = profileId;
		ContentCategoryId = contentCategoryId;
		Priority = priority;
	}
}
