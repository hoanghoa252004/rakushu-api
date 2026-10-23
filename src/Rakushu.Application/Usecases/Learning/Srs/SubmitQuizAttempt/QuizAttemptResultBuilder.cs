using System.Text.Json;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

internal static class QuizAttemptResultBuilder
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	public static Result<QuizAttemptResultResponse> Build(
		QuizAttempt attempt,
		IReadOnlyDictionary<Guid, QuizAnswerSubmission> answersByItemId,
		DateTimeOffset now,
		ISrsCardRepository cardRepository)
	{
		var questionResults = new List<QuizQuestionResultDto>();

		foreach (var item in attempt.Items.OrderBy(item => item.Position))
		{
			using var questionDocument = JsonDocument.Parse(item.QuestionSnapshot);
			using var correctAnswerDocument = JsonDocument.Parse(item.CorrectAnswerSnapshot);
			using var choicesDocument = JsonDocument.Parse(item.ChoicesSnapshot ?? "[]");
			using var feedbackDocument = JsonDocument.Parse(item.FeedbackSnapshot ?? "{}");

			var correctOptionId = correctAnswerDocument.RootElement
				.GetProperty("optionId")
				.GetString()!;

			var prompt = questionDocument.RootElement
				.GetProperty("prompt")
				.GetString() ?? string.Empty;

			var reading = questionDocument.RootElement.TryGetProperty("reading", out var readingElement)
				? readingElement.GetString()
				: null;

			var choices = JsonSerializer.Deserialize<List<QuizChoiceDto>>(
				choicesDocument.RootElement.GetRawText(),
				JsonOptions) ?? [];

			var feedback = feedbackDocument.RootElement.TryGetProperty("adaptive", out var adaptiveElement)
				? adaptiveElement.GetString() ?? string.Empty
				: feedbackDocument.RootElement.TryGetProperty(item.IsCorrect == true ? "correct" : "incorrect", out var feedbackElement)
					? feedbackElement.GetString() ?? string.Empty
					: string.Empty;

			if (attempt.AttemptMode == "SrsReview")
			{
				var card = item.SrsCard;
				var state = card.State;

				if (state is null)
				{
					return Result.Failure<QuizAttemptResultResponse>(
						SrsCardErrors.CardStateMissing);
				}

				var previousDueAt = state.DueAt;
				var previousIntervalDays = state.IntervalDays;
				var previousEaseFactor = state.EasinessFactor;
				var previousRepetitions = state.Repetitions;

				var submittedElapsedMs = answersByItemId[item.Id.Value].ElapsedMs;
				var elapsedMs = QuizAnswerQualityScorer.NormalizeElapsedMs(submittedElapsedMs);
				var quality = QuizAnswerQualityScorer.Calculate(
					item.IsCorrect == true,
					elapsedMs,
					!string.IsNullOrWhiteSpace(item.SelectedAnswer));
				var rating = SrsReviewRating.FromQuality(quality);
				var reviewResult = card.RecordReview(quality, now);

				if (reviewResult.IsFailure)
				{
					return Result.Failure<QuizAttemptResultResponse>(reviewResult.Error);
				}

				var reviewLog = SrsReviewLog.Create(
					card.Id,
					null,
					item.Id,
					rating,
					quality,
					now,
					elapsedMs,
					previousDueAt,
					state.DueAt,
					previousIntervalDays,
					state.IntervalDays,
					previousEaseFactor,
					state.EasinessFactor,
					previousRepetitions,
					state.Repetitions,
					now);

				cardRepository.AddReviewLog(reviewLog);
			}

			questionResults.Add(new QuizQuestionResultDto(
				item.Id.Value,
				item.Position,
				item.QuestionType,
				prompt,
				reading,
				item.SelectedAnswer ?? string.Empty,
				correctOptionId,
				item.IsCorrect == true,
				feedback,
				choices,
				item.SrsCard.State?.DueAt));
		}

		return Result.Success(new QuizAttemptResultResponse(
			attempt.Id.Value,
			attempt.Status.ToString(),
			(int)Math.Round(attempt.Score ?? 0m),
			attempt.TotalItems,
			attempt.CorrectItems,
			attempt.CompletedAt!.Value,
			questionResults));
	}
}
