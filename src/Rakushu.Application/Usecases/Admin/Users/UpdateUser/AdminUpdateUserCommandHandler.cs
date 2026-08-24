using MediatR;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;
using DomainProfile = Rakushu.Domain.Entities.Profile;

namespace Rakushu.Application.Usecases.Admin.Users.UpdateUser;

internal sealed class AdminUpdateUserCommandHandler : IRequestHandler<AdminUpdateUserCommand, Result<UserDetailDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly IProfileRepository _profileRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AdminUpdateUserCommandHandler(
		IUserRepository userRepository,
		IRoleRepository roleRepository,
		IProfileRepository profileRepository,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_profileRepository = profileRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<UserDetailDto>> Handle(AdminUpdateUserCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdWithProfileAndRoleAsync(request.UserId, cancellationToken);
		if (user is null)
		{
			return Result.Failure<UserDetailDto>(DomainErrors.User.NotFound);
		}

		if (!await _userRepository.IsEmailUniqueAsync(request.Email, excludeUserId: user.Id, cancellationToken: cancellationToken))
		{
			return Result.Failure<UserDetailDto>(DomainErrors.User.EmailAlreadyExists);
		}

		if (!await _userRepository.IsUsernameUniqueAsync(request.Username, excludeUserId: user.Id, cancellationToken: cancellationToken))
		{
			return Result.Failure<UserDetailDto>(DomainErrors.User.UsernameAlreadyExists);
		}

		var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
		if (role is null)
		{
			return Result.Failure<UserDetailDto>(DomainErrors.User.RoleNotFound);
		}

		user.UpdateAdmin(request.Username, request.Email, role.Id, request.Status);

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

		return Result.Success(new UserDetailDto(
			UserId: user.Id,
			Username: user.Username,
			Email: user.Email,
			RoleId: user.RoleId,
			RoleName: role.RoleName,
			DisplayName: profile.DisplayName,
			AvatarUrl: profile.AvatarUrl,
			Bio: profile.Bio,
			NativeLanguage: profile.NativeLanguage,
			LearningLanguage: profile.LearningLanguage,
			Status: user.Status,
			CreatedAt: user.CreatedAt,
			UpdatedAt: user.UpdatedAt
		));
	}
}
