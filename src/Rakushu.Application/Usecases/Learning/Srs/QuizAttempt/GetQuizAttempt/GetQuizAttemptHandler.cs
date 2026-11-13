using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttempt;

internal sealed class GetQuizAttemptHandler : IRequestHandler<GetQuizAttemptQuery, Result<QuizAttemptDetailDto>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;

	public GetQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IQuizAttemptSrsService quizAttemptSrsService,
		IUnitOfWork unitOfWork,
		IStorageService storageService)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_quizAttemptSrsService = quizAttemptSrsService;
		_unitOfWork = unitOfWork;
		_storageService = storageService;
	}

	public async Task<Result<QuizAttemptDetailDto>> Handle(
		GetQuizAttemptQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptDetailDto>(UserErrors.NotFound);

		var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(
			QuizAttemptId.From(request.AttemptId),
			cancellationToken);

		if (attempt is null || attempt.UserId != userId)
			return Result.Failure<QuizAttemptDetailDto>(QuizAttemptErrors.NotFound);

		var now = _systemClock.UtcNow;
		if (attempt.IsExpired(now))
		{
			attempt.Expire(now);
			if (attempt.HasAnyAnsweredItems)
			{
				await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);
			}
			_quizAttemptRepository.Update(attempt);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		bool isFinished = attempt.Status == QuizAttemptStatus.Completed || attempt.Status == QuizAttemptStatus.Expired || attempt.Status == QuizAttemptStatus.Canceled;

		var answeredItemsMap = attempt.Items.ToDictionary(i => i.QuestionId);
		var questionDtos = new List<QuizQuestionDto>(attempt.Quiz.Questions.Count);
		var presignedUrlCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		foreach (var question in attempt.Quiz.Questions.OrderBy(q => q.Sequence))
		{
			answeredItemsMap.TryGetValue(question.Id, out var item);

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

			var rawMetadata = QuestionMediaMetadataParser.Parse(question.MediaMetadata);
			var mediaMetadataDto = isFinished ? rawMetadata : QuestionMediaMetadataParser.SanitizeForActiveQuiz(rawMetadata);

			bool isSkip = item != null && QuizAnswerHelper.IsSkip(item.SelectedAnswer);
			bool? isItemCorrect = item?.IsCorrect;
			int timeSpentMs = item?.TimeSpentMs ?? 0;

			int? quality = null;
			if (isFinished && item != null && item.AnsweredAt.HasValue)
			{
				quality = isSkip ? 1
					: isItemCorrect != true ? 2
					: timeSpentMs > 15_000 ? 3
					: timeSpentMs >= 5_000 ? 4
					: 5;
			}

			questionDtos.Add(new QuizQuestionDto(
				ItemId: question.Id.Value,
				SavedItemId: question.SavedItemId.Value,
				Sequence: question.Sequence,
				Question: question.QuestionText,
				Choices: choices,
				SelectedAnswer: item?.SelectedAnswer,
				TimeSpentMs: timeSpentMs,
				QuestionType: typeCode,
				CorrectAnswer: isFinished ? question.CorrectAnswer : null,
				IsCorrect: isFinished ? isItemCorrect : null,
				Quality: quality,
				Feedback: isFinished ? item?.Feedback : null,
				Explanation: isFinished ? question.BankQuestion?.Explanation : null,
				MediaType: question.MediaType,
				MediaUrl: resolvedMediaUrl,
				MediaMetadata: mediaMetadataDto));
		}

		return Result.Success(new QuizAttemptDetailDto(
			Id: attempt.Id.Value,
			Mode: attempt.Mode.ToString(),
			Status: attempt.Status.ToString(),
			Score: isFinished ? attempt.Score : null,
			TotalItems: attempt.TotalItems,
			AnsweredItems: attempt.AnsweredItems,
			CorrectItems: isFinished ? attempt.CorrectItems : null,
			CreatedAt: attempt.CreatedAt,
			ExpiresAt: attempt.ExpiresAt,
			CompletedAt: attempt.CompletedAt,
			IsEvaluated: attempt.IsEvaluated,
			Questions: questionDtos));
	}
}
