using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttemptHandler
    : IRequestHandler<RetryQuizAttemptCommand, Result<QuizAttemptStartedResponse>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ICurrentUserContext _currentUser;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemClock _clock;

    public RetryQuizAttemptHandler(
        ICurrentUserContext currentUser,
        IQuizAttemptRepository attemptRepository,
        IUnitOfWork unitOfWork,
        ISystemClock clock)
    {
        _currentUser = currentUser;
        _attemptRepository = attemptRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<QuizAttemptStartedResponse>> Handle(
        RetryQuizAttemptCommand request,
        CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var userId = _currentUser.UserId;
            if (userId is null)
            {
                return Result.Failure<QuizAttemptStartedResponse>(
                    UserErrors.UnauthorizedResourceAccess);
            }

            var sourceAttempt = await _attemptRepository.GetForUserAsync(
                QuizAttemptId.From(request.AttemptId),
                userId,
                cancellationToken);

            if (sourceAttempt is null)
            {
                return Result.Failure<QuizAttemptStartedResponse>(
                    QuizAttemptErrors.QuizAttemptNotFound);
            }

            var retryResult = QuizAttempt.CreateRetryFrom(sourceAttempt, _clock.UtcNow);
            if (retryResult.IsFailure)
            {
                return Result.Failure<QuizAttemptStartedResponse>(retryResult.Error);
            }

            var retryAttempt = retryResult.Value;
            var questions = new List<QuizQuestionDto>();

            foreach (var item in retryAttempt.Items.OrderBy(item => item.Position))
            {
                using var questionDocument = JsonDocument.Parse(item.QuestionSnapshot);

                var prompt = questionDocument.RootElement
                    .GetProperty("prompt")
                    .GetString() ?? string.Empty;

                var reading = questionDocument.RootElement.TryGetProperty("reading", out var readingElement)
                    ? readingElement.GetString()
                    : null;

                var choices = JsonSerializer.Deserialize<List<QuizChoiceDto>>(
                    item.ChoicesSnapshot ?? "[]",
                    JsonOptions) ?? [];

                questions.Add(new QuizQuestionDto(
                    item.Id.Value,
                    item.Position,
                    item.QuestionType,
                    prompt,
                    reading,
                    choices));
            }

            _attemptRepository.Add(retryAttempt);

            return Result.Success(new QuizAttemptStartedResponse(
                retryAttempt.Id.Value,
                retryAttempt.Status.ToString(),
                retryAttempt.AttemptMode,
                retryAttempt.StartedAt,
                questions));
        }, cancellationToken);
    }
}