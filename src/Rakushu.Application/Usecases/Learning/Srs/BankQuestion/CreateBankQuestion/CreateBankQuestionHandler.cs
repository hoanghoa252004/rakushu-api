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
		bool correctAnswerExists = request.Answers.Any(c =>
			string.Equals(c.Id.Trim(), request.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase));

		if (!correctAnswerExists)
		{
			return Result.Failure<Guid>(BankQuestionErrors.ChoiceNotFound);
		}

		bool duplicateChoiceIds = request.Answers
			.GroupBy(c => c.Id.Trim(), StringComparer.OrdinalIgnoreCase)
			.Any(g => g.Count() > 1);

		if (duplicateChoiceIds)
		{
			return Result.Failure<Guid>(BankQuestionErrors.DuplicateChoiceId);
		}

		bool duplicateChoiceTexts = request.Answers
			.GroupBy(c => c.Text.Trim(), StringComparer.OrdinalIgnoreCase)
			.Any(g => g.Count() > 1);

		if (duplicateChoiceTexts)
		{
			return Result.Failure<Guid>(BankQuestionErrors.DuplicateChoiceText);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var meaningId = KnowledgeMeaningId.From(request.KnowledgeMeaningId);
			var meaning = await _knowledgeMeaningRepository.GetByIdAsync(meaningId, cancellationToken);
			if (meaning is null || !meaning.IsActive)
			{
				return Result.Failure<Guid>(BankQuestionErrors.MeaningNotFound);
			}

			bool questionTypeExists = await _bankQuestionRepository.QuestionTypeExistsAsync(request.QuestionTypeId, cancellationToken);
			if (!questionTypeExists)
			{
				return Result.Failure<Guid>(BankQuestionErrors.QuestionTypeNotFound);
			}

			var existing = await _bankQuestionRepository.FindDuplicateAsync(
				meaningId,
				request.QuestionTypeId,
				request.Question,
				cancellationToken);

			if (existing is not null)
			{
				if (existing.IsActive)
				{
					return Result.Failure<Guid>(BankQuestionErrors.DuplicateQuestion);
				}

				// If previously soft-deleted, reactivate and refresh with latest answers and details
				var reactivateAnswersPayload = request.Answers.Select(c => new
				{
					id = c.Id.Trim(),
					text = c.Text.Trim()
				});

				string reactivateAnswersJson = JsonSerializer.Serialize(reactivateAnswersPayload);
				var reactivateNow = _systemClock.UtcNow;

				existing.Reactivate(
					reactivateAnswersJson,
					request.CorrectAnswer,
					request.Explanation,
					request.Difficulty,
					reactivateNow);

				return Result.Success(existing.Id.Value);
			}

			var answersPayload = request.Answers.Select(c => new
			{
				id = c.Id.Trim(),
				text = c.Text.Trim()
			});

			string answersJson = JsonSerializer.Serialize(answersPayload);
			var now = _systemClock.UtcNow;

			var createResult = Domain.Entities.Srs.BankQuestion.BankQuestion.Create(
				meaningId,
				QuestionTypeId.From(request.QuestionTypeId),
				request.Question,
				answersJson,
				request.CorrectAnswer,
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
