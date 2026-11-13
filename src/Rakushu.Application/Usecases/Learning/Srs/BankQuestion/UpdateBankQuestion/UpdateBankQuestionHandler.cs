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
		string? answersJson = null;

		if (request.Answers != null && request.Answers.Count > 0)
		{
			bool correctAnswerExists = request.Answers.Any(c =>
				string.Equals(c.Id.Trim(), request.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase) ||
				string.Equals(c.Text.Trim(), request.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase));

			if (!correctAnswerExists)
			{
				return Result.Failure(BankQuestionErrors.ChoiceNotFound);
			}

			bool duplicateChoiceIds = request.Answers
				.GroupBy(c => c.Id.Trim(), StringComparer.OrdinalIgnoreCase)
				.Any(g => g.Count() > 1);

			if (duplicateChoiceIds)
			{
				return Result.Failure(BankQuestionErrors.DuplicateChoiceId);
			}

			bool duplicateChoiceTexts = request.Answers
				.GroupBy(c => c.Text.Trim(), StringComparer.OrdinalIgnoreCase)
				.Any(g => g.Count() > 1);

			if (duplicateChoiceTexts)
			{
				return Result.Failure(BankQuestionErrors.DuplicateChoiceText);
			}

			var answersPayload = request.Answers.Select(c => new
			{
				id = c.Id.Trim(),
				text = c.Text.Trim()
			});

			answersJson = JsonSerializer.Serialize(answersPayload);
		}
		else if (request.Answers != null && request.Answers.Count == 0)
		{
			answersJson = "[]";
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var questionId = BankQuestionId.From(request.Id);
			var question = await _bankQuestionRepository.GetByIdAsync(questionId, cancellationToken);
			if (question is null)
			{
				return Result.Failure(BankQuestionErrors.NotFound);
			}

			bool isDuplicate = await _bankQuestionRepository.ExistsDuplicateAsync(
				question.KnowledgeMeaningId,
				question.QuestionTypeId.Value,
				request.Question,
				excludeId: questionId,
				cancellationToken: cancellationToken);

			if (isDuplicate)
			{
				return Result.Failure(BankQuestionErrors.DuplicateQuestion);
			}

			var now = _systemClock.UtcNow;

			if (question.Status == QuestionStatus.Draft)
			{
				question.UpdateAndPublish(
					request.Question,
					answersJson ?? question.Answers,
					request.CorrectAnswer,
					request.Explanation,
					now,
					request.MediaType,
					request.MediaUrl,
					request.MediaMetadata);
			}
			else
			{
				question.Update(
					request.Question,
					answersJson ?? question.Answers,
					request.CorrectAnswer,
					request.Explanation,
					now,
					request.MediaType,
					request.MediaUrl,
					request.MediaMetadata);
			}

			_bankQuestionRepository.Update(question);
			return Result.Success();
		}, cancellationToken);
	}
}