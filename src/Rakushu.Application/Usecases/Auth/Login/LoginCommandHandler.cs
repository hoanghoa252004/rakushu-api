using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Auth.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IUnitOfWork _unitOfWork;

	public LoginCommandHandler(
		IUserRepository userRepository,
		IPasswordHasher passwordHasher,
		IJwtTokenGenerator jwtTokenGenerator,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
		_jwtTokenGenerator = jwtTokenGenerator;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<LoginResponseDto>> Handle(
		LoginCommand request,
		CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (user is null)
		{
			return Result.Failure<LoginResponseDto>(UserError.InvalidCredentials);
		}

		if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
		{
			return Result.Failure<LoginResponseDto>(UserError.InvalidCredentials);
		}

		if (user.Status != UserStatus.Active)
		{
			return Result.Failure<LoginResponseDto>(UserError.UserInactive);
		}

		var roleName = user.Role?.RoleName ?? "Learner";
		var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Username, roleName);
		var (refreshTokenStr, expiresAt) = _jwtTokenGenerator.GenerateRefreshToken();

		var refreshToken = user.AddRefreshToken(refreshTokenStr, expiresAt);
		_userRepository.AddRefreshToken(refreshToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success(new LoginResponseDto(
			user.Id,
			user.Username,
			user.Email,
			roleName,
			user.Profile?.DisplayName ?? user.Username,
			user.Profile?.AvatarUrl,
			accessToken,
			refreshTokenStr,
			expiresAt));
	}
}