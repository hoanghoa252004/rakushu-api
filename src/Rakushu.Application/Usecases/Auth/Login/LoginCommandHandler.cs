using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Enums;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;
using DomainRefreshToken = Rakushu.Domain.Entities.RefreshToken;

namespace Rakushu.Application.Usecases.Auth.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IUnitOfWork _unitOfWork;

	public LoginCommandHandler(
		IUserRepository userRepository,
		IRefreshTokenRepository refreshTokenRepository,
		IPasswordHasher passwordHasher,
		IJwtTokenGenerator jwtTokenGenerator,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_refreshTokenRepository = refreshTokenRepository;
		_passwordHasher = passwordHasher;
		_jwtTokenGenerator = jwtTokenGenerator;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (user is null)
		{
			return Result.Failure<LoginResponseDto>(DomainErrors.Auth.InvalidCredentials);
		}

		if (user.Status == UserStatus.Banned.ToString() || user.Status == UserStatus.Inactive.ToString())
		{
			return Result.Failure<LoginResponseDto>(DomainErrors.Auth.UserInactive);
		}

		if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
		{
			return Result.Failure<LoginResponseDto>(DomainErrors.Auth.InvalidCredentials);
		}

		var roleName = user.Role?.RoleName ?? "User";
		var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, roleName);
		var refreshTokenStr = _jwtTokenGenerator.GenerateRefreshToken();
		var expiresAt = DateTimeOffset.UtcNow.AddDays(_jwtTokenGenerator.GetRefreshTokenExpirationDays());
		var refreshToken = DomainRefreshToken.Create(user.Id, refreshTokenStr, expiresAt);

		_refreshTokenRepository.Add(refreshToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success(new LoginResponseDto(
			UserId: user.Id,
			Username: user.Username,
			Email: user.Email,
			Role: roleName,
			DisplayName: user.Profile?.DisplayName ?? user.Username,
			AvatarUrl: user.Profile?.AvatarUrl,
			AccessToken: accessToken,
			RefreshToken: refreshTokenStr,
			AccessTokenExpiresAt: DateTimeOffset.UtcNow.AddMinutes(_jwtTokenGenerator.GetAccessTokenExpirationMinutes())
		));
	}
}