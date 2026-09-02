using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Profile.GetMyProfile;

internal sealed class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<ProfileResponseDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUserRepository _userRepository;

	public GetMyProfileQueryHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
	}

	public async Task<Result<ProfileResponseDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (!userId.HasValue)
		{
			return Result.Failure<ProfileResponseDto>(UserError.InvalidCredentials);
		}

		var user = await _userRepository.GetByIdWithDetailsAsync(userId.Value, cancellationToken);
		if (user is null)
		{
			return Result.Failure<ProfileResponseDto>(UserError.NotFound);
		}

		var profile = user.Profile;
		return Result.Success(new ProfileResponseDto(
			UserId: user.Id,
			Username: user.Username,
			Email: user.Email,
			Role: user.Role?.RoleName ?? "Learner",
			DisplayName: profile?.DisplayName ?? user.Username,
			AvatarUrl: profile?.AvatarUrl,
			Bio: profile?.Bio,
			NativeLanguage: profile?.NativeLanguage,
			LearningLanguage: profile?.LearningLanguage,
			Status: user.Status.ToString(),
			CreatedAt: profile?.CreatedAt ?? user.CreatedAt,
			UpdatedAt: profile?.UpdatedAt ?? user.UpdatedAt
		));
	}
}
