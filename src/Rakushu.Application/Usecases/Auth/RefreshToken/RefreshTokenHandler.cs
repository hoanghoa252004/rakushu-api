using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Auth.Login;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Auth.RefreshToken;

internal sealed class RefreshTokenHandler
	: IRequestHandler<RefreshTokenCommand, Result<CredentialResponseDto>>
{

	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly ISystemClock _systemClock;

	public RefreshTokenHandler(
		IUserRepository userRepository,
		IUnitOfWork unitOfWork,
		IJwtTokenGenerator jwtTokenGenerator,
		ISystemClock systemClock
		)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_jwtTokenGenerator = jwtTokenGenerator;
		_systemClock = systemClock;
	}

	public async Task<Result<CredentialResponseDto>> Handle(
		RefreshTokenCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Find the user
			var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

			if (user == null) // Check whether the user exists
			{
				return Result.Failure<CredentialResponseDto>(UserError.NotFound);
			}
			else if (user.Status != UserStatus.Active) // Check whether the user is active
			{
				return Result.Failure<CredentialResponseDto>(UserError.UserInactiveOrBannned);
			}

			// 2. Validate refresh token
			var refreshToken = user.RefreshTokens
			.SingleOrDefault(rt => request.RefreshToken == rt.TokenHash
							&& rt.IsActive == true 
							&& rt.UsedAt == null);
			if(refreshToken == null)
			{
				return Result.Failure<CredentialResponseDto>(UserError.InvalidRefreshToken);
			}

			var refreshTokenUsedAt = _systemClock.UtcNow;

			refreshToken.Revoke(refreshTokenUsedAt);

			// 3. Generate a new access token and refresh token

			var utcNow = _systemClock.UtcNow;

			// ----- 3.1 Generate Access Token
			var accessTokenLifetime = _jwtTokenGenerator.GetAccessTokenExpirationMinutes();

			var accessTokenExpiresAt = utcNow.AddMinutes(accessTokenLifetime);

			var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Role.Title, accessTokenExpiresAt);

			// ----- 3.2 Generate Refresh Token
			var hashedToken = _jwtTokenGenerator.GenerateHashedToken();

			var refreshTokenLifetime = _jwtTokenGenerator.GetRefreshTokenExpirationDays();

			var refreshTokenExpiresAt = utcNow.AddDays(refreshTokenLifetime);

			var newRefreshToken = user.AddRefreshToken(user.Id, hashedToken, utcNow, refreshTokenExpiresAt);

			// 5. Return LoginResponseDto
			return Result.Success(new CredentialResponseDto(
				accessToken,
				accessTokenExpiresAt,
				newRefreshToken.TokenHash,
				refreshTokenExpiresAt
			));
		}, cancellationToken);
	}
}
