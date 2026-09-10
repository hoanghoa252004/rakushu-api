using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Auth.Login;

internal sealed class LoginHandler : IRequestHandler<LoginCommand, Result<CredentialResponseDto>>
{
	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;

	public LoginHandler(
		IUserRepository userRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		IPasswordHasher passwordHasher,
		IJwtTokenGenerator jwtTokenGenerator
		)
	{
		_userRepository = userRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
		_passwordHasher = passwordHasher;
		_jwtTokenGenerator = jwtTokenGenerator;
	}

	public async Task<Result<CredentialResponseDto>> Handle(
		LoginCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Check Email
			var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
			if (user is null)
			{
				return Result.Failure<CredentialResponseDto>(UserError.InvalidCredentials);
			}

			// 2. Check Password
			if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
			{
				return Result.Failure<CredentialResponseDto>(UserError.InvalidCredentials);
			}

			// 3. Check User Status
			if (user.Status == UserStatus.Unverified)
			{
				return Result.Failure<CredentialResponseDto>(UserError.UnverifiedYet);
			}

			if (user.Status != UserStatus.Active)
			{
				return Result.Failure<CredentialResponseDto>(UserError.UserInactiveOrBannned);
			}

			// 4. Generate Access Token and Refresh Token
			var role = user.Role;

			var utcNow = _systemClock.UtcNow;

			// ----- 4.1 Generate Access Token
			var accessTokenLifetime = _jwtTokenGenerator.GetAccessTokenExpirationMinutes();

			var accessTokenExpiresAt = utcNow.AddMinutes(accessTokenLifetime);

			var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Role.Title, accessTokenExpiresAt);

			// ----- 4.2 Generate Refresh Token
			var hashedToken = _jwtTokenGenerator.GenerateHashedToken();

			var refreshTokenLifetime = _jwtTokenGenerator.GetRefreshTokenExpirationDays();

			var refreshTokenExpiresAt = utcNow.AddDays(refreshTokenLifetime);

			var refreshToken = user.AddRefreshToken(user.Id, hashedToken, utcNow, refreshTokenExpiresAt);

			// 5. Return LoginResponseDto
			return Result.Success(new CredentialResponseDto(
				accessToken,
				accessTokenExpiresAt,
				refreshToken.TokenHash,
				refreshTokenExpiresAt
			));
		}, cancellationToken);
	}
}