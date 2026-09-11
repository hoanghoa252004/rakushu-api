using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Email;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.EmailVerificationToken;
using Rakushu.Domain.Entities.User.ValueObjects.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Auth.SendEmailVerificationCode;

internal sealed class SendEmailVerificationCodeHandler : IRequestHandler<SendEmailVerificationCodeCommand, Result>
{
	// DAOs
	private readonly IUserRepository _userRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IVerificationCodeHasher _verificationCodeHasher;
	private readonly IEmailService _emailSender;

	public SendEmailVerificationCodeHandler(
		IUserRepository userRepository, 
		IUnitOfWork unitOfWork,
		IEmailService emailSender,
		IVerificationCodeHasher verificationCodeHasher
		)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_emailSender = emailSender;
		_verificationCodeHasher = verificationCodeHasher;
	}

	public async Task<Result> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync( async () =>
		{
			// 1. Find user by email
			var emailResult = Email.Create(request.Email);

			if (emailResult.IsFailure)
			{
				return emailResult;
			}	

			var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

			// Don't reveal whether email exists.
			if (user == null)
			{
				return Result.Failure(UserError.NotFound);
			}

			if (user.Status != UserStatus.Unverified)
			{
				return Result.Failure(UserError.NoNeedToVerify);
			}

			// 2. Check whether any verification token active:
			var now = DateTimeOffset.UtcNow;

			var activeToken = user.EmailVerificationTokens
					.SingleOrDefault(t => !t.IsUsed && !t.IsExpired(now));

			if (activeToken != null)
			{
				var remaining = activeToken.ExpiresAt - now;

				return Result.Failure(
					UserError.RemainActiveVerificationCode($"Please wait {Math.Ceiling(remaining.TotalMinutes)} minutes."));
			}

			// Generate code
			var code = _verificationCodeHasher.Generate6DigitCode();

			// Hash code
			var codeHash = _verificationCodeHasher.Hash(code);

			var token = EmailVerificationToken.Create(
				user.Id,
				codeHash,
				now,
				_verificationCodeHasher.GetExpirationTime(now));

			user.AddEmailVerificationToken(token);

			await _emailSender.SendAsync(
				user.Email.Value,
				"VERIFY EMAIL RAKUSHU SYSTEM",
				code,
				cancellationToken);

			return Result.Success();
		}, cancellationToken);
	}
}
