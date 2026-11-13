using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttemptResult;

internal sealed class GetQuizAttemptResultHandler : IRequestHandler<GetQuizAttemptResultQuery, Result<QuizAttemptResultDto>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IStorageService _storageService;

	public GetQuizAttemptResultHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		IStorageService storageService)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
	}

	public async Task<Result<QuizAttemptResultDto>> Handle(
		GetQuizAttemptResultQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptResultDto>(UserErrors.NotFound);

		var attemptId = QuizAttemptId.From(request.AttemptId);
		var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
		if (attempt is null || attempt.UserId != userId)
			return Result.Failure<QuizAttemptResultDto>(QuizAttemptErrors.NotFound);

		if (attempt.Status != QuizAttemptStatus.Completed && attempt.Status != QuizAttemptStatus.Expired)
			return Result.Failure<QuizAttemptResultDto>(QuizAttemptErrors.NotCompleted);

		var questionsDict = attempt.Quiz.Questions.ToDictionary(q => q.Id);
		var answerResults = new List<QuizAnswerResultDto>(attempt.Items.Count);
		var presignedUrlCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		var orderedItems = attempt.Items
			.Select(i => new { Item = i, Question = questionsDict.GetValueOrDefault(i.QuestionId) })
			.Where(x => x.Question is not null)
			.OrderBy(x => x.Question!.Sequence);

		foreach (var entry in orderedItems)
		{
			var item = entry.Item;
			var question = entry.Question!;

			bool isSkip = QuizAnswerHelper.IsSkip(item.SelectedAnswer);
			bool isCorrect = item.IsCorrect ?? false;
			int timeSpentMs = item.TimeSpentMs;

			int quality = isSkip ? 1
				: !isCorrect ? 2
				: timeSpentMs > 15_000 ? 3
				: timeSpentMs >= 5_000 ? 4
				: 5;

			var typeCode = question.QuestionType?.Code;
			var isTyping = string.Equals(typeCode, "TYPE_READING", StringComparison.OrdinalIgnoreCase) ||
			               string.Equals(typeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase) ||
			               string.Equals(typeCode, "AUDIO_DICTATION", StringComparison.OrdinalIgnoreCase);

			IReadOnlyList<QuizOptionDto> choices = isTyping ? [] : QuizOptionParser.Parse(question.Answers);

			string? resolvedMediaUrl = question.MediaUrl;
			if (!string.IsNullOrWhiteSpace(resolvedMediaUrl) &&
			    !resolvedMediaUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
			    !resolvedMediaUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				if (presignedUrlCache.TryGetValue(resolvedMediaUrl, out var cachedUrl))
				{
					resolvedMediaUrl = cachedUrl;
				}
				else
				{
					try
					{
						var signed = await _storageService.CreatePresignedReadUrlAsync(resolvedMediaUrl, cancellationToken);
						presignedUrlCache[resolvedMediaUrl] = signed;
						resolvedMediaUrl = signed;
					}
					catch
					{
						// Fallback to raw key
					}
				}
			}

			var mediaMetadataDto = QuestionMediaMetadataParser.Parse(question.MediaMetadata);

			answerResults.Add(new QuizAnswerResultDto(
				ItemId: question.Id.Value,
				SavedItemId: question.SavedItemId.Value,
				Sequence: question.Sequence,
				Question: question.QuestionText,
				Choices: choices,
				SelectedAnswer: item.SelectedAnswer,
				CorrectAnswer: question.CorrectAnswer,
				IsCorrect: isCorrect,
				TimeSpentMs: timeSpentMs,
				Quality: quality,
				Feedback: item.Feedback,
				Explanation: question.BankQuestion?.Explanation,
				QuestionType: typeCode,
				MediaType: question.MediaType,
				MediaUrl: resolvedMediaUrl,
				MediaMetadata: mediaMetadataDto));
		}

		return Result.Success(new QuizAttemptResultDto(
			Id: attempt.Id.Value,
			QuizId: attempt.QuizId.Value,
			Mode: attempt.Mode.ToString(),
			Status: attempt.Status.ToString(),
			Score: attempt.Score ?? 0,
			TotalItems: attempt.TotalItems,
			AnsweredItems: attempt.AnsweredItems,
			CorrectItems: attempt.CorrectItems,
			CreatedAt: attempt.CreatedAt,
			CompletedAt: attempt.CompletedAt,
			IsEvaluated: attempt.IsEvaluated,
			Answers: answerResults));
	}
}
