using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Auth.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IUnitOfWork _unitOfWork;

	public ChangePasswordCommandHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		IPasswordHasher passwordHasher,
		IRefreshTokenRepository refreshTokenRepository,
		IUnitOfWork unitOfWork)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
		_refreshTokenRepository = refreshTokenRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (!userId.HasValue)
		{
			return Result.Failure(DomainErrors.Auth.InvalidCredentials);
		}

		var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
		if (user is null)
		{
			return Result.Failure(DomainErrors.User.NotFound);
		}

		if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
		{
			return Result.Failure(DomainErrors.Auth.PasswordMismatch);
		}

		if (request.CurrentPassword == request.NewPassword)
		{
			return Result.Failure(DomainErrors.Auth.SamePassword);
		}

		var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
		user.UpdatePassword(newPasswordHash);

		// Revoke previous refresh tokens for security
		await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
