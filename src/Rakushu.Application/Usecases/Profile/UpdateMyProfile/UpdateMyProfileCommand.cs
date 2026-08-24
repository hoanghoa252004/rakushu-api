using MediatR;
using Rakushu.Application.Usecases.Profile.GetMyProfile;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Profile.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(
	string DisplayName,
	string? AvatarUrl = null,
	string? Bio = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
) : IRequest<Result<ProfileResponseDto>>;
