using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Enums;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;
using DomainProfile = Rakushu.Domain.Entities.Profile;

namespace Rakushu.Application.Usecases.Admin.Users.CreateUser;

internal sealed class AdminCreateUserCommandHandler : IRequestHandler<AdminCreateUserCommand, Result<UserDetailDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly IProfileRepository _profileRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IUnitOfWork _unitOfWork;

	public AdminCreateUserCommandHandler(
		IUserRepository userRepository,
		IRoleRepository roleRepository,
		IProfileRepository profileRepository,
		IPasswordHasher passwordHasher,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_profileRepository = profileRepository;
		_passwordHasher = passwordHasher;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<UserDetailDto>> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
	{
		if (!await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken: cancellationToken))
		{
			return Result.Failure<UserDetailDto>(DomainErrors.User.EmailAlreadyExists);
		}

		if (!await _userRepository.IsUsernameUniqueAsync(request.Username, cancellationToken: cancellationToken))
		{
			return Result.Failure<UserDetailDto>(DomainErrors.User.UsernameAlreadyExists);
		}

		var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
		if (role is null)
		{
			return Result.Failure<UserDetailDto>(DomainErrors.User.RoleNotFound);
		}

		var userId = Guid.NewGuid();
		var passwordHash = _passwordHasher.HashPassword(request.Password);
		var status = string.IsNullOrWhiteSpace(request.Status) ? UserStatus.Active.ToString() : request.Status;

		var user = User.Create(
			username: request.Username,
			email: request.Email,
			passwordHash: passwordHash,
			roleId: role.Id,
			status: status,
			id: userId);

		var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
			? request.Username
			: request.DisplayName.Trim();

		var profile = DomainProfile.Create(
			userId: userId,
			displayName: displayName,
			avatarUrl: null,
			bio: null,
			nativeLanguage: request.NativeLanguage,
			learningLanguage: request.LearningLanguage);

		_userRepository.Add(user);
		_profileRepository.Add(profile);

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
