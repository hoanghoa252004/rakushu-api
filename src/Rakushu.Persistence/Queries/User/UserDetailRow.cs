using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Queries.User;

internal sealed record UserDetailRow(
	Guid Id,
	string Email,
	string Status,
	DateTime CreatedAt,
	DateTime UpdatedAt,

	// Role data
	Guid RoleId,
	string RoleCode,
	string RoleName,


	// Profile data
	Guid? ProfileId,
	string? FullName,
	string? Avatar,
	int? DailyLearningMinutes,
	int? SessionDurationMinutes,

	// Native language data
	Guid? NativeLanguageId,
	string? NativeLanguageCode,
	string? NativeLanguageName,
	string? NativeLanguageNativeName,

	// Current level data
	Guid? CurrentLevelId,
	string? CurrentLevelCode,
	string? CurrentLevelName,
	Guid? CurrentFrameworkId,
	string? CurrentFrameworkCode,
	string? CurrentFrameworkName,

	// Target level data
	Guid? TargetLevelId,
	string? TargetLevelCode,
	string? TargetLevelName,
	Guid? TargetFrameworkId,
	string? TargetFrameworkCode,
	string? TargetFrameworkName
);

internal sealed record UserInterestRow(
	Guid Id,
	int Priority,

	// Content category data
	Guid ContentCategoryId,
	string ContentCategorySlug,
	string ContentCategoryCode,
	string ContentCategoryName
);	