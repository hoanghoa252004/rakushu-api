namespace Rakushu.Application.Usecases.Profile.GetMyProfile;

public sealed record ProfileResponseDto(
	Guid UserId,
	string Username,
	string Email,
	string Role,
	string DisplayName,
	string? AvatarUrl,
	string? Bio,
	string? NativeLanguage,
	string? LearningLanguage,
	string Status,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
);
