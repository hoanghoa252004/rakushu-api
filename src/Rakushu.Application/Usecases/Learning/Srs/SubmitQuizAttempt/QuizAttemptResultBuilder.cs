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
		DateTimeOffset now)
	{
		var questionResults = new List<QuizQuestionResultDto>();

		foreach (var item in attempt.Items.OrderBy(item => item.Position))
		{
			var question = item.Question;
			var correctOptionId = question.CorrectOptionId;
			var prompt = question.Prompt;
			var reading = question.Reading;
			var choices = JsonSerializer.Deserialize<List<QuizChoiceDto>>(
				question.ChoicesJson,
				JsonOptions) ?? [];

			var srsItem = item.SrsItem;
			var submittedElapsedMs = answersByItemId[item.Id.Value].ElapsedMs;
			var elapsedMs = QuizAnswerQualityScorer.NormalizeElapsedMs(submittedElapsedMs);
			var isCorrect = item.IsCorrect == true;
			var quality = item.Quality ?? QuizAnswerQualityScorer.Calculate(
				isCorrect,
				elapsedMs,
				!string.IsNullOrWhiteSpace(item.SelectedOptionId));

			// Only update SM-2 schedule during official SRS Review mode
			if (attempt.Mode == QuizAttemptMode.SrsReview)
			{
				var reviewResult = srsItem.RecordReview(quality, now);
				if (reviewResult.IsFailure)
				{
					return Result.Failure<QuizAttemptResultResponse>(reviewResult.Error);
				}
			}

			var feedback = item.Feedback ?? (isCorrect
				? "Chính xác! Bạn đã chọn đúng đáp án."
				: "Chưa chính xác.");

			questionResults.Add(new QuizQuestionResultDto(
				item.Id.Value,
				item.Position,
				question.QuestionType,
				prompt,
				reading,
				item.SelectedOptionId ?? string.Empty,
				correctOptionId,
				isCorrect,
				feedback,
				choices,
				srsItem.DueAt));
		}

		return Result.Success(new QuizAttemptResultResponse(
			attempt.Id.Value,
			attempt.Status.ToString(),
			(int)Math.Round(attempt.Score ?? 0m),
			attempt.TotalItems,
			attempt.CorrectItems,
			attempt.CompletedAt!.Value,
			questionResults,
			attempt.Mode.ToString()));
	}
}
