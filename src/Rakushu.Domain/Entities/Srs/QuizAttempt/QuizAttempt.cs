using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public sealed class QuizAttempt : AggregateRoot<QuizAttemptId>
{
    public UserId UserId { get; private set; } = null!;
    public QuizAttemptId? SourceAttemptId { get; private set; }
    public string TemplateCode { get; private set; } = "WORD_TO_MEANING_MCQ";
    public string AttemptMode { get; private set; } = "Practice";
    public QuizAttemptStatus Status { get; private set; } = QuizAttemptStatus.Created;
    public int TotalItems { get; private set; }
    public int AnsweredItems { get; private set; }
    public int CorrectItems { get; private set; }
    public decimal? Score { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public long? ElapsedMs { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public User User { get; private set; } = null!;
    public QuizAttempt? SourceAttempt { get; private set; }
    public ICollection<QuizAttemptItem> Items { get; private set; } = new List<QuizAttemptItem>();

    private QuizAttempt() { }
    private QuizAttempt(QuizAttemptId id) : base(id) { }

    public static Result<QuizAttempt> Create(UserId userId, string templateCode, string attemptMode, DateTimeOffset now, QuizAttemptId? sourceAttemptId = null)
    {
        if (userId is null)
            return Result.Failure<QuizAttempt>(QuizAttemptErrors.InvalidUserId);
        if (string.IsNullOrWhiteSpace(templateCode))
            return Result.Failure<QuizAttempt>(QuizAttemptErrors.InvalidQuizAnswer);
        if (attemptMode is not ("Practice" or "SrsReview"))
            return Result.Failure<QuizAttempt>(QuizAttemptErrors.InvalidQuizAnswer);

        return Result.Success(new QuizAttempt(QuizAttemptId.Create())
        {
            UserId = userId, TemplateCode = templateCode, AttemptMode = attemptMode,
            SourceAttemptId = sourceAttemptId, Status = QuizAttemptStatus.InProgress,
            StartedAt = now, CreatedAt = now, UpdatedAt = now
        });
    }

    public static Result<QuizAttempt> CreateRetryFrom(QuizAttempt source, DateTimeOffset now)
    {
        if (source is null || source.Status != QuizAttemptStatus.Completed)
            return Result.Failure<QuizAttempt>(QuizAttemptErrors.RetryRequiresCompletedAttempt);
        var retryResult = Create(source.UserId, source.TemplateCode, "Practice", now, source.Id);
        if (retryResult.IsFailure)
            return retryResult;
        var retry = retryResult.Value;
        foreach (var sourceItem in source.Items.OrderBy(x => x.Position))
        {
            var itemResult = sourceItem.CreateRetryCopy(now);
            if (itemResult.IsFailure)
                return Result.Failure<QuizAttempt>(itemResult.Error);
            var addResult = retry.AddItem(itemResult.Value);
            if (addResult.IsFailure)
                return Result.Failure<QuizAttempt>(addResult.Error);
        }
        return Result.Success(retry);
    }

    public Result AddItem(QuizAttemptItem item)
    {
        if (Status != QuizAttemptStatus.InProgress)
            return Result.Failure(QuizAttemptErrors.QuizNotInProgress);
        if (item is null || item.SrsCardId.Value == Guid.Empty)
            return Result.Failure(QuizAttemptErrors.InvalidCardId);
        if (Items.Any(x => x.Position == item.Position) || Items.Any(x => x.SrsCardId == item.SrsCardId))
            return Result.Failure(QuizAttemptErrors.DuplicateQuizCard);
        item.AssignToAttempt(Id, Items.Count + 1);
        Items.Add(item);
        TotalItems = Items.Count;
        UpdatedAt = item.CreatedAt;
        return Result.Success();
    }

    public Result RecordAnswer(Guid itemId, string selectedAnswer, bool isCorrect, DateTimeOffset answeredAt, long? elapsedMs = null)
    {
        if (Status != QuizAttemptStatus.InProgress)
            return Result.Failure(QuizAttemptErrors.QuizNotInProgress);
        var item = Items.SingleOrDefault(x => x.Id.Value == itemId);
        if (item is null)
            return Result.Failure(QuizAttemptErrors.QuizItemNotFound);
        var answerResult = item.RecordAnswer(selectedAnswer, isCorrect, answeredAt, elapsedMs);
        if (answerResult.IsFailure)
            return answerResult;
        AnsweredItems = Items.Count(x => x.AnsweredAt is not null);
        CorrectItems = Items.Count(x => x.IsCorrect == true);
        UpdatedAt = answeredAt;
        return Result.Success();
    }

    public Result Complete(DateTimeOffset completedAt)
    {
        if (Status != QuizAttemptStatus.InProgress)
            return Result.Failure(QuizAttemptErrors.QuizNotInProgress);
        if (TotalItems == 0 || AnsweredItems != TotalItems)
            return Result.Failure(QuizAttemptErrors.QuizHasUnansweredItems);
        Status = QuizAttemptStatus.Completed;
        CompletedAt = completedAt;
        ElapsedMs = Math.Max(0, (long)(completedAt - StartedAt).TotalMilliseconds);
        Score = Math.Round((decimal)CorrectItems * 100m / TotalItems, 2);
        UpdatedAt = completedAt;
        return Result.Success();
    }

    public Result Abandon(DateTimeOffset abandonedAt)
    {
        if (Status != QuizAttemptStatus.InProgress)
            return Result.Failure(QuizAttemptErrors.QuizNotInProgress);
        Status = QuizAttemptStatus.Abandoned;
        UpdatedAt = abandonedAt;
        return Result.Success();
    }
}