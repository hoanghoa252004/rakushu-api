using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.User.User.GetUserById;

public sealed record UserDetailDto(
	Guid Id,
	string FullName,
	string Email,
	string Status,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
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
	string? Avatar,
	int DailyLearningMinutes,
	int SessionDurationMinutes,
	LevelDto Level,
	IReadOnlyCollection<InterestDto> Interests
);


public sealed record LevelDto(
	Guid Id,
	string Code,
	string Name,
	string JapaneseName,
	string? Description
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
	string Name,
	string JapaneseName,
	string ThemeColor,
	string? Description
);