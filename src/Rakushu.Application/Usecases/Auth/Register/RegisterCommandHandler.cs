using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;
using UserProfile = Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.Auth.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponseDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IUnitOfWork _unitOfWork;

	public RegisterCommandHandler(
		IUserRepository userRepository,
		IRoleRepository roleRepository,
		IPasswordHasher passwordHasher,
		IJwtTokenGenerator jwtTokenGenerator,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_passwordHasher = passwordHasher;
		_jwtTokenGenerator = jwtTokenGenerator;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<RegisterResponseDto>> Handle(
		RegisterCommand request,
		CancellationToken cancellationToken)
	{
		// 1. Check unique email
		if (!await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken: cancellationToken))
		{
			return Result.Failure<RegisterResponseDto>(UserError.EmailAlreadyExists);
		}

		// 2. Check unique username
		if (!await _userRepository.IsUsernameUniqueAsync(request.Username, cancellationToken: cancellationToken))
		{
			return Result.Failure<RegisterResponseDto>(UserError.UsernameAlreadyExists);
		}

		// 3. Find default Learner role
		var role = await _roleRepository.GetByNameAsync(RoleConstants.Learner, cancellationToken)
			?? await _roleRepository.GetByIdAsync(RoleConstants.LearnerRoleId, cancellationToken);

		if (role is null)
		{
			return Result.Failure<RegisterResponseDto>(RoleError.NotFound);
		}

		// 4. Create User & Profile
		var passwordHash = _passwordHasher.HashPassword(request.Password);
		var user = User.Create(
			username: request.Username,
			email: request.Email,
			passwordHash: passwordHash,
			roleId: role.Id,
			status: UserStatus.Active);

		var profile = UserProfile.Create(
			userId: user.Id,
			displayName: request.DisplayName ?? request.Username,
			nativeLanguage: "Vietnamese",
			learningLanguage: "Japanese");

		user.SetProfile(profile);
		_userRepository.Add(user);

		// 5. Generate Tokens
		var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Username, role.RoleName);
		var (refreshTokenStr, expiresAt) = _jwtTokenGenerator.GenerateRefreshToken();

		var refreshToken = user.AddRefreshToken(refreshTokenStr, expiresAt);
		_userRepository.AddRefreshToken(refreshToken);

		// 6. Save to Database
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success(new RegisterResponseDto(
			user.Id,
			user.Username,
			user.Email,
			role.RoleName,
			profile.DisplayName ?? user.Username,
			accessToken,
			refreshTokenStr,
			expiresAt));
	}
}
