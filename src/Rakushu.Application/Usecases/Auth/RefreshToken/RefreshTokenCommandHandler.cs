using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Enums;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;
using DomainRefreshToken = Rakushu.Domain.Entities.RefreshToken;

namespace Rakushu.Application.Usecases.Auth.RefreshToken;

internal sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
{
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IUnitOfWork _unitOfWork;

	public RefreshTokenCommandHandler(
		IRefreshTokenRepository refreshTokenRepository,
		IJwtTokenGenerator jwtTokenGenerator,
		IUnitOfWork unitOfWork)
	{
		_refreshTokenRepository = refreshTokenRepository;
		_jwtTokenGenerator = jwtTokenGenerator;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
	{
		var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
		if (token is null || !token.IsActive)
		{
			return Result.Failure<RefreshTokenResponseDto>(DomainErrors.Auth.InvalidRefreshToken);
		}

		var user = token.User;
		if (user is null || user.Status == UserStatus.Banned.ToString() || user.Status == UserStatus.Inactive.ToString())
		{
			return Result.Failure<RefreshTokenResponseDto>(DomainErrors.Auth.UserInactive);
		}

		// Rotate refresh token
		token.Revoke();

		var roleName = user.Role?.RoleName ?? "User";
		var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user, roleName);
		var newRefreshTokenStr = _jwtTokenGenerator.GenerateRefreshToken();
		var expiresAt = DateTimeOffset.UtcNow.AddDays(_jwtTokenGenerator.GetRefreshTokenExpirationDays());
		var newRefreshToken = DomainRefreshToken.Create(user.Id, newRefreshTokenStr, expiresAt);

		_refreshTokenRepository.Add(newRefreshToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success(new RefreshTokenResponseDto(
			AccessToken: newAccessToken,
			RefreshToken: newRefreshTokenStr,
			AccessTokenExpiresAt: DateTimeOffset.UtcNow.AddMinutes(_jwtTokenGenerator.GetAccessTokenExpirationMinutes())
		));
	}
}
