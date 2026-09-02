using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Auth.RefreshToken;

internal sealed class RefreshTokenCommandHandler
	: IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IUnitOfWork _unitOfWork;

	public RefreshTokenCommandHandler(
		IUserRepository userRepository,
		IJwtTokenGenerator jwtTokenGenerator,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_jwtTokenGenerator = jwtTokenGenerator;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<RefreshTokenResponseDto>> Handle(
		RefreshTokenCommand request,
		CancellationToken cancellationToken)
	{
		var existingToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
		if (existingToken is null || !existingToken.IsActive)
		{
			return Result.Failure<RefreshTokenResponseDto>(UserError.InvalidRefreshToken);
		}

		var user = await _userRepository.GetByIdWithDetailsAsync(existingToken.UserId, cancellationToken);
		if (user is null || user.Status != UserStatus.Active)
		{
			return Result.Failure<RefreshTokenResponseDto>(UserError.UserInactive);
		}

		// Rotate token: revoke old, create new
		existingToken.Revoke();

		var roleName = user.Role?.RoleName ?? "Learner";
		var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Username, roleName);
		var (newRefreshTokenStr, expiresAt) = _jwtTokenGenerator.GenerateRefreshToken();

		var newRefreshToken = user.AddRefreshToken(newRefreshTokenStr, expiresAt);
		_userRepository.AddRefreshToken(newRefreshToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success(new RefreshTokenResponseDto(
			newAccessToken,
			newRefreshTokenStr,
			expiresAt));
	}
}
