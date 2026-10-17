namespace Rakushu.Application.Usecases.Learning.Srs.EnrollLearningUnit;

public sealed record EnrollLearningUnitResponse(
    Guid NoteId,
    Guid CardId,
    string Status,
    DateTimeOffset DueAt,
    bool AlreadyEnrolled);
