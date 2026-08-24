using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Constants;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Enums;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;
using DomainProfile = Rakushu.Domain.Entities.Profile;
using DomainRefreshToken = Rakushu.Domain.Entities.RefreshToken;

namespace Rakushu.Application.Usecases.Auth.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponseDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	private readonly IProfileRepository _profileRepository;
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IUnitOfWork _unitOfWork;

	public RegisterCommandHandler(
		IUserRepository userRepository,
		IRoleRepository roleRepository,
		IProfileRepository profileRepository,
		IRefreshTokenRepository refreshTokenRepository,
		IPasswordHasher passwordHasher,
		IJwtTokenGenerator jwtTokenGenerator,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_profileRepository = profileRepository;
		_refreshTokenRepository = refreshTokenRepository;
		_passwordHasher = passwordHasher;
		_jwtTokenGenerator = jwtTokenGenerator;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<RegisterResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		// 1. Check unique email
		if (!await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken: cancellationToken))
		{
			return Result.Failure<RegisterResponseDto>(DomainErrors.User.EmailAlreadyExists);
		}

		// 2. Check unique username
		if (!await _userRepository.IsUsernameUniqueAsync(request.Username, cancellationToken: cancellationToken))
		{
			return Result.Failure<RegisterResponseDto>(DomainErrors.User.UsernameAlreadyExists);
		}

		// 3. Find default User role
		var role = await _roleRepository.GetByNameAsync(RoleConstants.User, cancellationToken)
			?? await _roleRepository.GetByIdAsync(RoleConstants.UserRoleId, cancellationToken);

		if (role is null)
		{
			return Result.Failure<RegisterResponseDto>(DomainErrors.User.RoleNotFound);
		}

		// 4. Create User & Profile
		var userId = Guid.NewGuid();
		var passwordHash = _passwordHasher.HashPassword(request.Password);
		var user = User.Create(
			username: request.Username,
			email: request.Email,
			passwordHash: passwordHash,
			roleId: role.Id,
			status: UserStatus.Active.ToString(),
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

		// 5. Generate Tokens
		var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, role.RoleName);
		var refreshTokenStr = _jwtTokenGenerator.GenerateRefreshToken();
		var expiresAt = DateTimeOffset.UtcNow.AddDays(_jwtTokenGenerator.GetRefreshTokenExpirationDays());
		var refreshToken = DomainRefreshToken.Create(userId, refreshTokenStr, expiresAt);

		_refreshTokenRepository.Add(refreshToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success(new RegisterResponseDto(
			UserId: user.Id,
			Username: user.Username,
			Email: user.Email,
			Role: role.RoleName,
			DisplayName: profile.DisplayName,
			AccessToken: accessToken,
			RefreshToken: refreshTokenStr,
			AccessTokenExpiresAt: DateTimeOffset.UtcNow.AddMinutes(_jwtTokenGenerator.GetAccessTokenExpirationMinutes())
		));
	}
}
