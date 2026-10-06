using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory.Specifications;
using Rakushu.Domain.Entities.ProficiencyLevel.Specifications;
using Rakushu.Domain.Entities.User.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.Profile.Policies;

public sealed class ProfileUpdatePolicy
{
	private readonly ActiveUserSpecification
		_activeUserSpecification;

	private readonly ActiveProficiencyLevelSpecification
		_activeLevelSpecification;

	private readonly ActiveContentCategorySpecification
		_activeCategorySpecification;

	public ProfileUpdatePolicy(
		ActiveUserSpecification activeUserSpecification,
		ActiveProficiencyLevelSpecification activeLevelSpecification,
		ActiveContentCategorySpecification activeCategorySpecification)
	{
		_activeUserSpecification = activeUserSpecification;
		_activeLevelSpecification = activeLevelSpecification;
		_activeCategorySpecification = activeCategorySpecification;
	}

	public Result Validate(
		User user,
		ProficiencyLevel.ProficiencyLevel level,
		IReadOnlyCollection<ContentCategory.ContentCategory> interestCategories)
	{
		// 0. User active
		var userResult = _activeUserSpecification.IsSatisfiedBy(user);

		if(userResult.IsFailure)
		{
			return userResult;
		}

		// 2. Levels active
		var levelResult = _activeLevelSpecification.IsSatisfiedBy(level);

		if (levelResult.IsFailure)
		{
			return levelResult;
		}

		// 5. Categories active
		foreach (var category in interestCategories)
		{
			var categoryResult = _activeCategorySpecification.IsSatisfiedBy(category);

			if (categoryResult.IsFailure)
			{
				return categoryResult;
			}
		}

		return Result.Success();
	}
}