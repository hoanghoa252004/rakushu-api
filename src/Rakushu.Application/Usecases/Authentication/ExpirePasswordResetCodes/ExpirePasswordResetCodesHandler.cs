using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User.PasswordResetCode;

namespace Rakushu.Application.Usecases.Authentication.ExpirePasswordResetCodes;

internal sealed class ExpirePasswordResetCodesHandler : IRequestHandler<ExpirePasswordResetCodesCommand, Result>
{
	private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public ExpirePasswordResetCodesHandler(
		IPasswordResetCodeRepository passwordResetCodeRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_passwordResetCodeRepository = passwordResetCodeRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(ExpirePasswordResetCodesCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var codes = await _passwordResetCodeRepository.GetNotYetFlaggedExpiredAsync(now, cancellationToken);

			foreach (var code in codes)
			{
				code.MarkAsExpired();
			}

			return Result.Success();
		}, cancellationToken);
	}
}
