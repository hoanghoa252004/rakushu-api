using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.UpdateBankQuestion;

internal sealed class UpdateBankQuestionHandler : IRequestHandler<UpdateBankQuestionCommand, Result>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateBankQuestionHandler(
		IBankQuestionRepository bankQuestionRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_bankQuestionRepository = bankQuestionRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		UpdateBankQuestionCommand request,
		CancellationToken cancellationToken)
	{
		bool correctChoiceExists = request.Choices.Any(c =>
			string.Equals(c.Id.Trim(), request.CorrectChoiceId.Trim(), StringComparison.OrdinalIgnoreCase));

		if (!correctChoiceExists)
		{
			return Result.Failure(BankQuestionErrors.ChoiceNotFound);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var questionId = BankQuestionId.From(request.Id);
			var question = await _bankQuestionRepository.GetByIdAsync(questionId, cancellationToken);
			if (question is null)
			{
				return Result.Failure(BankQuestionErrors.NotFound);
			}

			var choicesPayload = request.Choices.Select(c => new
			{
				id = c.Id.Trim(),
				text = c.Text.Trim()
			});

			string choicesJson = JsonSerializer.Serialize(choicesPayload);
			var now = _systemClock.UtcNow;

			question.Update(
				request.Prompt,
				choicesJson,
				request.CorrectChoiceId,
				request.Explanation,
				request.Difficulty,
				now);

			return Result.Success();
		}, cancellationToken);
	}
}
