using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

namespace Rakushu.Application.Usecases.Authentication.ResetPassword;

internal sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IVerificationCodeHasher _verificationCodeHasher;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ISystemClock _systemClock;

	public ResetPasswordHandler(
		IUserRepository userRepository,
		IPasswordResetCodeRepository passwordResetCodeRepository,
		IUnitOfWork unitOfWork,
		IVerificationCodeHasher verificationCodeHasher,
		IPasswordHasher passwordHasher,
		ISystemClock systemClock)
	{
		_userRepository = userRepository;
		_passwordResetCodeRepository = passwordResetCodeRepository;
		_unitOfWork = unitOfWork;
		_verificationCodeHasher = verificationCodeHasher;
		_passwordHasher = passwordHasher;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Find user by email
			var user = await _userRepository.GetByEmailAsync(request.Email.ToLower(), cancellationToken);

			if (user == null)
			{
				return Result.Failure(UserErrors.NotFound);
			}

			// 2. Find and validate code
			var codeHash = _verificationCodeHasher.Hash(request.Code);

			var resetCode = await _passwordResetCodeRepository.GetByUserIdAndCodeHashAsync(user.Id, codeHash, cancellationToken);

			var now = _systemClock.UtcNow;

			if (resetCode == null
				|| !resetCode.IsActive(now)
				|| !_verificationCodeHasher.Verify(request.Code, resetCode.CodeHash))
			{
				return Result.Failure(UserErrors.InvalidPasswordResetCode);
			}

			// 3. Update password
			if (_passwordHasher.VerifyPassword(request.NewPassword, user.PasswordHash))
			{
				return Result.Failure(UserErrors.SamePassword);
			}

			user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));

			// 4. Revoke sessions & mark code as used
			user.RevokeAllActiveRefreshTokens();

			resetCode.MarkAsUsed(now);

			return Result.Success();
		}, cancellationToken);
	}
}
