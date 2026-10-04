using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory.Specifications;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.Specifications;
using Rakushu.Domain.Entities.SupportedLanguage.Specifications;
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
	private readonly ActiveSupportedLanguageSpecification
		_activeLanguageSpecification;

	private readonly ActiveProficiencyLevelSpecification
		_activeLevelSpecification;

	private readonly SameFrameworkSpecification
		_sameFrameworkSpecification;

	private readonly ActiveContentCategorySpecification
		_activeCategorySpecification;

	public ProfileUpdatePolicy(
		ActiveUserSpecification activeUserSpecification,
		ActiveSupportedLanguageSpecification activeLanguageSpecification,
		ActiveProficiencyLevelSpecification activeLevelSpecification,
		SameFrameworkSpecification sameFrameworkSpecification,
		ActiveContentCategorySpecification activeCategorySpecification)
	{
		_activeUserSpecification = activeUserSpecification;
		_activeLanguageSpecification = activeLanguageSpecification;
		_sameFrameworkSpecification = sameFrameworkSpecification;
		_activeLevelSpecification = activeLevelSpecification;
		_activeCategorySpecification = activeCategorySpecification;
	}

	public Result Validate(
		User user,
		SupportedLanguage.SupportedLanguage nativeLanguage,
		ProficiencyLevel currentLevel,
		ProficiencyLevel targetLevel,
		IReadOnlyCollection<ContentCategory.ContentCategory> interestCategories)
	{
		// 0. User active
		var userResult = _activeUserSpecification.IsSatisfiedBy(user);

		if(userResult.IsFailure)
		{
			return userResult;
		}

		// 1. Language active
		var languageResult = _activeLanguageSpecification.IsSatisfiedBy(nativeLanguage);

		if (languageResult.IsFailure)
		{
			return languageResult;
		}

		// 2. Levels active
		var currentLevelResult = _activeLevelSpecification.IsSatisfiedBy(currentLevel);

		if (currentLevelResult.IsFailure)
		{
			return currentLevelResult;
		}

		var targetLevelResult = _activeLevelSpecification.IsSatisfiedBy(targetLevel);

		if (targetLevelResult.IsFailure)
		{
			return targetLevelResult;
		}

		// 3. Same framework
		var sameFrameworkResult = _sameFrameworkSpecification.IsSatisfiedBy(
			new ProficiencyLevelPair(currentLevel, targetLevel));

		if (sameFrameworkResult.IsFailure)
		{
			return sameFrameworkResult;
		}

		// 4. Current level lower than target level
		if(currentLevel.SortOrder >= targetLevel.SortOrder)
		{
			return Result.Failure(ProfileErrors.CurrentNotHigherThanTargetLevel);
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