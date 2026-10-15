namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsReviewLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SrsCardId { get; set; }
    public Guid? SrsSessionItemId { get; set; }
    public Guid? QuizAttemptItemId { get; set; }
    public string Rating { get; set; } = null!;
    public DateTimeOffset ReviewedAt { get; set; }
    public int? ElapsedMs { get; set; }
    public DateTimeOffset? PreviousDueAt { get; set; }
    public DateTimeOffset? NextDueAt { get; set; }
    public int? PreviousIntervalDays { get; set; }
    public int? NextIntervalDays { get; set; }
    public double? PreviousEasinessFactor { get; set; }
    public double? NextEasinessFactor { get; set; }
    public int? PreviousRepetitions { get; set; }
    public int? NextRepetitions { get; set; }
    public string SchedulerVersion { get; set; } = "SM2-v1";
    public DateTimeOffset CreatedAt { get; set; }
    public SrsCard SrsCard { get; set; } = null!;
}
