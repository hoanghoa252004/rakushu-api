using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.GetQuizAttemptResult;

internal sealed class GetQuizAttemptResultHandler
	: IRequestHandler<GetQuizAttemptResultQuery, Result<QuizAttemptResultResponse>>
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private readonly ICurrentUserContext _currentUser;
	private readonly IQuizAttemptRepository _attemptRepository;

	public GetQuizAttemptResultHandler(
		ICurrentUserContext currentUser,
		IQuizAttemptRepository attemptRepository)
	{
		_currentUser = currentUser;
		_attemptRepository = attemptRepository;
	}

	public async Task<Result<QuizAttemptResultResponse>> Handle(
		GetQuizAttemptResultQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				UserErrors.UnauthorizedResourceAccess);
		}

		var attempt = await _attemptRepository.GetForUserAsync(
			QuizAttemptId.From(request.AttemptId),
			userId,
			cancellationToken);

		if (attempt is null)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				QuizAttemptErrors.QuizAttemptNotFound);
		}

		if (attempt.Status != QuizAttemptStatus.Completed)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				QuizAttemptErrors.QuizNotInProgress);
		}

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

			questionResults.Add(new QuizQuestionResultDto(
				item.Id.Value,
				item.Position,
				item.QuestionType,
				prompt,
				reading,
				item.SelectedAnswer!,
				correctOptionId,
				item.IsCorrect == true,
				feedback,
				choices));
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