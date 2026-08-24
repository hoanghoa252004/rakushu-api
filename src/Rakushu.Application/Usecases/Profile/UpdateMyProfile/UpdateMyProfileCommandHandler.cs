using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Application.Usecases.Profile.GetMyProfile;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;
using DomainProfile = Rakushu.Domain.Entities.Profile;

namespace Rakushu.Application.Usecases.Profile.UpdateMyProfile;

internal sealed class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, Result<ProfileResponseDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUserRepository _userRepository;
	private readonly IProfileRepository _profileRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateMyProfileCommandHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		IProfileRepository profileRepository,
		IUnitOfWork unitOfWork)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_profileRepository = profileRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<ProfileResponseDto>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (!userId.HasValue)
		{
			return Result.Failure<ProfileResponseDto>(DomainErrors.Auth.InvalidCredentials);
		}

		var user = await _userRepository.GetByIdWithProfileAndRoleAsync(userId.Value, cancellationToken);
		if (user is null)
		{
			return Result.Failure<ProfileResponseDto>(DomainErrors.User.NotFound);
		}

		var profile = user.Profile;
		if (profile is null)
		{
			profile = DomainProfile.Create(
				userId: user.Id,
				displayName: request.DisplayName,
				avatarUrl: request.AvatarUrl,
				bio: request.Bio,
				nativeLanguage: request.NativeLanguage,
				learningLanguage: request.LearningLanguage);

			_profileRepository.Add(profile);
		}
		else
		{
			profile.Update(
				displayName: request.DisplayName,
				avatarUrl: request.AvatarUrl,
				bio: request.Bio,
				nativeLanguage: request.NativeLanguage,
				learningLanguage: request.LearningLanguage);
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success(new ProfileResponseDto(
			UserId: user.Id,
			Username: user.Username,
			Email: user.Email,
			Role: user.Role?.RoleName ?? "User",
			DisplayName: profile.DisplayName,
			AvatarUrl: profile.AvatarUrl,
			Bio: profile.Bio,
			NativeLanguage: profile.NativeLanguage,
			LearningLanguage: profile.LearningLanguage,
			Status: user.Status,
			CreatedAt: profile.CreatedAt,
			UpdatedAt: profile.UpdatedAt
		));
	}
}
