using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Email;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

namespace Rakushu.Application.Usecases.Authentication.SendPasswordResetCode;

internal sealed class SendPasswordResetCodeHandler : IRequestHandler<SendPasswordResetCodeCommand, Result>
{
	private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(15);

	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IVerificationCodeHasher _verificationCodeHasher;
	private readonly IEmailService _emailSender;
	private readonly ISystemClock _systemClock;

	public SendPasswordResetCodeHandler(
		IUserRepository userRepository,
		IPasswordResetCodeRepository passwordResetCodeRepository,
		IUnitOfWork unitOfWork,
		IVerificationCodeHasher verificationCodeHasher,
		IEmailService emailSender,
		ISystemClock systemClock)
	{
		_userRepository = userRepository;
		_passwordResetCodeRepository = passwordResetCodeRepository;
		_unitOfWork = unitOfWork;
		_verificationCodeHasher = verificationCodeHasher;
		_emailSender = emailSender;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(SendPasswordResetCodeCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Find user by email
			var user = await _userRepository.GetByEmailAsync(request.Email.ToLower(), cancellationToken);

			if (user == null)
			{
				return Result.Failure(UserErrors.NotFound);
			}

			var activeResult = user.IsActive();

			if (activeResult.IsFailure)
			{
				return activeResult;
			}

			// 2. Only allow a new code when previous codes are used or expired
			var now = _systemClock.UtcNow;

			var activeCode = await _passwordResetCodeRepository.GetActiveByUserIdAsync(user.Id, now, cancellationToken);

			if (activeCode != null)
			{
				var remaining = activeCode.ExpiresAt - now;

				return Result.Failure(
					UserErrors.RemainActivePasswordResetCode($"Please wait {Math.Ceiling(remaining.TotalMinutes)} minutes."));
			}

			// 3. Generate, hash and store code
			var code = _verificationCodeHasher.Generate6DigitCode();

			var resetCode = PasswordResetCode.Create(
				user.Id,
				_verificationCodeHasher.Hash(code),
				now,
				now.Add(CodeLifetime));

			_passwordResetCodeRepository.Add(resetCode);

			// 4. Send email
			await _emailSender.SendAsync(
				user.Email.ToLower(),
				"RESET PASSWORD RAKUSHU SYSTEM",
				code,
				cancellationToken);

			return Result.Success();
		}, cancellationToken);
	}
}
