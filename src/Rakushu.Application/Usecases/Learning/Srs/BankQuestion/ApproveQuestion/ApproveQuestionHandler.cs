using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.ApproveQuestion;

internal sealed class ApproveQuestionHandler : IRequestHandler<ApproveQuestionCommand, Result>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public ApproveQuestionHandler(
		IBankQuestionRepository bankQuestionRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_bankQuestionRepository = bankQuestionRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		ApproveQuestionCommand request,
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
			question.Publish(now);
			_bankQuestionRepository.Update(question);

			return Result.Success();
		}, cancellationToken);
	}
}
