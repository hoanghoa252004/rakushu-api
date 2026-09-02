using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Admin.Users.CreateUser;

internal sealed class AdminCreateUserCommandHandler : IRequestHandler<AdminCreateUserCommand, Result<UserDetailDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IUnitOfWork _unitOfWork;

	public AdminCreateUserCommandHandler(
		IUserRepository userRepository,
		IRoleRepository roleRepository,
		IPasswordHasher passwordHasher,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_passwordHasher = passwordHasher;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<UserDetailDto>> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
	{
		if (!await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken: cancellationToken))
		{
			return Result.Failure<UserDetailDto>(UserError.EmailAlreadyExists);
		}

		if (!await _userRepository.IsUsernameUniqueAsync(request.Username, cancellationToken: cancellationToken))
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
			: UserStatus.Active;

		var passwordHash = _passwordHasher.HashPassword(request.Password);

		var user = User.Create(
			username: request.Username,
			email: request.Email,
			passwordHash: passwordHash,
			roleId: role.Id,
			status: status);

		var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
			? request.Username
			: request.DisplayName.Trim();

		var profile = UserProfile.Create(
			userId: user.Id,
			displayName: displayName,
			avatarUrl: null,
			bio: null,
			nativeLanguage: request.NativeLanguage,
			learningLanguage: request.LearningLanguage);

		user.SetProfile(profile);
		_userRepository.Add(user);

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
