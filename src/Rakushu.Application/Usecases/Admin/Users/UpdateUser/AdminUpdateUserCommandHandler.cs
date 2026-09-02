using MediatR;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;
using UserProfile = Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.Admin.Users.UpdateUser;

internal sealed class AdminUpdateUserCommandHandler : IRequestHandler<AdminUpdateUserCommand, Result<UserDetailDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AdminUpdateUserCommandHandler(
		IUserRepository userRepository,
		IRoleRepository roleRepository,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<UserDetailDto>> Handle(AdminUpdateUserCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
		if (user is null)
		{
			return Result.Failure<UserDetailDto>(UserError.NotFound);
		}

		if (!await _userRepository.IsEmailUniqueAsync(request.Email, excludeUserId: user.Id, cancellationToken: cancellationToken))
		{
			return Result.Failure<UserDetailDto>(UserError.EmailAlreadyExists);
		}

		if (!await _userRepository.IsUsernameUniqueAsync(request.Username, excludeUserId: user.Id, cancellationToken: cancellationToken))
		{
			return Result.Failure<UserDetailDto>(UserError.UsernameAlreadyExists);
		}

		var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
		if (role is null)
		{
			return Result.Failure<UserDetailDto>(RoleError.NotFound);
		}

		var status = Enum.TryParse<UserStatus>(request.Status, true, out var parsedStatus)
			? parsedStatus
			: user.Status;

		user.UpdateAccount(request.Username, request.Email, role.Id, status);

		var profile = user.Profile;
		if (profile is null)
		{
			profile = UserProfile.Create(
				userId: user.Id,
				displayName: request.DisplayName,
				avatarUrl: request.AvatarUrl,
				bio: request.Bio,
				nativeLanguage: request.NativeLanguage,
				learningLanguage: request.LearningLanguage);

			user.SetProfile(profile);
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
			DisplayName: profile.DisplayName ?? user.Username,
			AvatarUrl: profile.AvatarUrl,
			Bio: profile.Bio,
			NativeLanguage: profile.NativeLanguage,
			LearningLanguage: profile.LearningLanguage,
			Status: user.Status.ToString(),
			CreatedAt: user.CreatedAt,
			UpdatedAt: user.UpdatedAt
		));
	}
}
