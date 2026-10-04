using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.User.User.GetUserById;

public sealed record UserDetailDto(
	Guid Id,
	string Email,
	string Status,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	RoleDto Role,
	ProfileDto? Profile
);

public sealed record RoleDto(
	Guid Id,
	string Code,
	string Name
);

public sealed record ProfileDto(
	Guid Id,
	string FullName,
	string? Avatar,
	int DailyLearningMinutes,
	int SessionDurationMinutes,
	NativeLanguageDto NativeLanguage,
	CurrentLevelDto CurrentLevel,
	TargetLevelDto TargetLevel,
	IReadOnlyCollection<InterestDto> Interests
);


public sealed record CurrentLevelDto(
	Guid Id,
	string Code,
	string Name,
	FrameworkLevelDto FrameworkLevel
);

public sealed record TargetLevelDto(
	Guid Id,
	string Code,
	string Name,
	FrameworkLevelDto FrameworkLevel
);

public sealed record FrameworkLevelDto(
	Guid Id,
	string Code,
	string Name
);

public sealed record NativeLanguageDto(
	Guid Id,
	string Code,
	string Name,
	string NativeName
);

public sealed record InterestDto(
	Guid Id,
	int Priority,
	InterestedContentDto Content
);

public sealed record InterestedContentDto(
	Guid Id,
	string Slug,
	string Code,
	string Name
);