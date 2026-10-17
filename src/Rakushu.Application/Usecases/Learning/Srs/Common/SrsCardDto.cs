using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.Common;

public sealed record SrsCardDto(
    Guid CardId,
    Guid NoteId,
    Guid LearningUnitId,
    string Expression,
    string Reading,
    string Meaning,
    DateTimeOffset DueAt,
    int Repetitions,
    int IntervalDays)
{
    public static SrsCardDto FromEntity(SrsCard entity) =>
        new(
            entity.Id.Value,
            entity.SrsNoteId.Value,
            entity.SrsNote.LearningUnitId.Value,
            entity.SrsNote.LearningUnit.KnowledgeMeaning.KnowledgeUnit.Expression,
            entity.SrsNote.LearningUnit.KnowledgeMeaning.KnowledgeUnit.Reading,
            entity.SrsNote.LearningUnit.KnowledgeMeaning.Meaning,
            entity.State!.DueAt,
            entity.State.Repetitions,
            entity.State.IntervalDays);
}
