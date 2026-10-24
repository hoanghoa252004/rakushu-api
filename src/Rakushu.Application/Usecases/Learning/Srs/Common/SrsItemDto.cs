using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.Common;

public sealed record SrsItemDto(
	Guid Id,
	Guid KnowledgeMeaningId,
	Guid? LearningUnitId,
	string Expression,
	string Reading,
	string Meaning,
	string? Description,
	string Status,
	DateTimeOffset DueAt,
	int IntervalDays,
	double EasinessFactor,
	int Repetitions,
	int Lapses,
	DateTimeOffset? LastReviewAt,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt)
{
	public static SrsItemDto FromEntity(SrsItem entity) =>
		new(
			entity.Id.Value,
			entity.KnowledgeMeaningId.Value,
			entity.SourceLearningUnitId?.Value,
			entity.Expression,
			entity.Reading ?? string.Empty,
			entity.Meaning,
			entity.Description,
			entity.Status.ToString(),
			entity.DueAt,
			entity.IntervalDays,
			entity.EasinessFactor,
			entity.Repetitions,
			entity.Lapses,
			entity.LastReviewAt,
			entity.CreatedAt,
			entity.UpdatedAt);
}
