using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Profile.UpdateProfile;

public sealed record UpdateProfileCommand(
	string FullName,
	string NativeLanguage,
	string? AvatarKey = null
) : IRequest<Result>;
