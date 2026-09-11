using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Email;
using Rakushu.Application.Usecases.Auth.SendEmailVerificationCode;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.ValueObjects.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Auth.Verify;


internal sealed class VerifyHandler : IRequestHandler<VerifyCommand, Result>
{
	// DAOs
	private readonly IUserRepository _userRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IVerificationCodeHasher _verificationCodeHasher;
	private readonly IEmailService _emailSender;

	public VerifyHandler(
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

	public async Task<Result> Handle(VerifyCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
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

			// 2. Find token
			var codeHash = _verificationCodeHasher.Hash(request.Code);

			var token = user.EmailVerificationTokens.SingleOrDefault(t => t.CodeHash == codeHash);

			if (token == null)
			{
				return Result.Failure(UserError.VerificationCodeNotFound);
			}

			var now = DateTimeOffset.UtcNow;

			if (token.IsUsed == true || token.IsExpired(now))
			{
				return Result.Failure(UserError.InvalidVerificationCode);

			}

			if(_verificationCodeHasher.Verify(request.Code, token.CodeHash) == false)
			{
				return Result.Failure(UserError.InvalidVerificationCode);
			}

			// 3. Update user status
			user.VerifyEmail();

			// 4. Mark email verification token as used:
			token.MarkAsUsed(now);

			return Result.Success();
		}, cancellationToken);
	}
}

