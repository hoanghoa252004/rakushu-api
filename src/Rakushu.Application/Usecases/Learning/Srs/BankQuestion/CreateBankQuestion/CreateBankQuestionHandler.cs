using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.CreateBankQuestion;

internal sealed class CreateBankQuestionHandler : IRequestHandler<CreateBankQuestionCommand, Result<Guid>>
{
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly IKnowledgeMeaningRepository _knowledgeMeaningRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateBankQuestionHandler(
		IBankQuestionRepository bankQuestionRepository,
		IKnowledgeMeaningRepository knowledgeMeaningRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_bankQuestionRepository = bankQuestionRepository;
		_knowledgeMeaningRepository = knowledgeMeaningRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		CreateBankQuestionCommand request,
		CancellationToken cancellationToken)
	{
		bool correctChoiceExists = request.Choices.Any(c =>
			string.Equals(c.Id.Trim(), request.CorrectChoiceId.Trim(), StringComparison.OrdinalIgnoreCase));

		if (!correctChoiceExists)
		{
			return Result.Failure<Guid>(BankQuestionErrors.ChoiceNotFound);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var meaningId = KnowledgeMeaningId.From(request.KnowledgeMeaningId);
			var meaning = await _knowledgeMeaningRepository.GetByIdAsync(meaningId, cancellationToken);
			if (meaning is null)
			{
				return Result.Failure<Guid>(BankQuestionErrors.MeaningNotFound);
			}

			bool questionTypeExists = await _bankQuestionRepository.QuestionTypeExistsAsync(request.QuestionTypeId, cancellationToken);
			if (!questionTypeExists)
			{
				return Result.Failure<Guid>(BankQuestionErrors.QuestionTypeNotFound);
			}

			var choicesPayload = request.Choices.Select(c => new
			{
				id = c.Id.Trim(),
				text = c.Text.Trim()
			});

			string choicesJson = JsonSerializer.Serialize(choicesPayload);
			var now = _systemClock.UtcNow;

			var createResult = Domain.Entities.Srs.BankQuestion.BankQuestion.Create(
				meaningId,
				QuestionTypeId.From(request.QuestionTypeId),
				request.Prompt,
				choicesJson,
				request.CorrectChoiceId,
				now,
				request.Explanation,
				request.Difficulty,
				request.IsActive);

			if (createResult.IsFailure)
			{
				return Result.Failure<Guid>(createResult.Error);
			}

			var bankQuestion = createResult.Value;
			_bankQuestionRepository.Add(bankQuestion);

			return Result.Success(bankQuestion.Id.Value);
		}, cancellationToken);
	}
}
