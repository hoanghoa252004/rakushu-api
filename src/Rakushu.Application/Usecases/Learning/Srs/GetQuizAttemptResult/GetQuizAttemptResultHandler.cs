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

	// CONTEXT
	private readonly ICurrentUserContext _currentUser;

	// REPOSITORIES
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
		// 1. Validate User Context
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				UserErrors.UnauthorizedResourceAccess);
		}

		// 2. Fetch Quiz Attempt
		var attempt = await _attemptRepository.GetForUserAsync(
			QuizAttemptId.From(request.AttemptId),
			userId,
			cancellationToken);

		if (attempt is null)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				QuizAttemptErrors.QuizAttemptNotFound);
		}

		// 3. Verify Completion Status
		if (attempt.Status != QuizAttemptStatus.Completed)
		{
			return Result.Failure<QuizAttemptResultResponse>(
				QuizAttemptErrors.AttemptNotCompleted);
		}

		// 4. Map Attempt Results
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

			var feedback = item.Feedback ?? (item.IsCorrect == true
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
				item.IsCorrect == true,
				feedback,
				choices,
				item.SrsItem?.DueAt));
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
