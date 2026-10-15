using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsNote : AggregateRoot<SrsNoteId>
{
    public UserId UserId { get; private set; } = null!;
    public LearningUnitId LearningUnitId { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }
    public User User { get; private set; } = null!;
    public LearningUnit LearningUnit { get; private set; } = null!;

    private SrsNote() { }

    private SrsNote(SrsNoteId id) : base(id) { }

    public static SrsNote Create(UserId userId, LearningUnitId learningUnitId, DateTimeOffset now) =>
        new(SrsNoteId.Create())
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId)),
            LearningUnitId = learningUnitId ?? throw new ArgumentNullException(nameof(learningUnitId)),
            CreatedAt = now,
            UpdatedAt = now
        };

    public bool IsArchived => ArchivedAt is not null;

    public void Archive(DateTimeOffset now)
    {
        ArchivedAt ??= now;
        UpdatedAt = now;
    }
}