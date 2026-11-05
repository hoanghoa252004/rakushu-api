using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.ToggleBankQuestionStatus;

internal sealed class ToggleBankQuestionStatusHandler : IRequestHandler<ToggleBankQuestionStatusCommand, Result>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public ToggleBankQuestionStatusHandler(
		IBankQuestionRepository bankQuestionRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_bankQuestionRepository = bankQuestionRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		ToggleBankQuestionStatusCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var questionId = BankQuestionId.From(request.Id);
			var question = await _bankQuestionRepository.GetByIdAsync(questionId, cancellationToken);
			if (question is null)
			{
				return Result.Failure(BankQuestionErrors.NotFound);
			}

			var now = _systemClock.UtcNow;
			if (request.IsActive)
			{
				question.Activate(now);
			}
			else
			{
				question.Deactivate(now);
			}

			return Result.Success();
		}, cancellationToken);
	}
}
