using FluentValidation;
using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.LearningUnit;

public sealed record LearningUnitKnowledgeCollectionDto(
	Guid Id,
	string Code,
	string Name,
	string CollectionType);

public sealed record LearningUnitKnowledgeUnitDto(
	Guid Id,
	string KnowledgeType,
	string Expression,
	string Reading,
	bool IsActive,
	LearningUnitKnowledgeCollectionDto KnowledgeCollection);

public sealed record LearningUnitMeaningDto(
	Guid Id,
	int SortOrder,
	string Meaning,
	string JapaneseMeaning,
	string? Description,
	bool IsUsed);

public sealed record LearningUnitSummaryDto(
	Guid Id,
	Guid TranscriptSegmentId,
	int StartTokenIndex,
	int EndTokenIndex,
	double Confidence,
	bool IsReviewed,
	string DetectionMethod,
	LearningUnitKnowledgeUnitDto KnowledgeUnit,
	LearningUnitMeaningDto KnowledgeMeaning);

public sealed record LearningUnitDto(
	Guid Id,
	Guid TranscriptSegmentId,
	int StartTokenIndex,
	int EndTokenIndex,
	double Confidence,
	bool IsReviewed,
	string DetectionMethod,
	LearningUnitKnowledgeUnitDto KnowledgeUnit,
	LearningUnitMeaningDto KnowledgeMeaning,
	int TotalMeaningCount,
	IReadOnlyList<LearningUnitMeaningDto> Meanings);

internal static class LearningUnitMapper2
{
	internal static LearningUnitKnowledgeUnitDto MapUnit(Domain.Entities.Knowledge.KnowledgeUnit.KnowledgeUnit u) =>
		new(
			u.Id.Value,
			u.KnowledgeType.ToString(),
			u.Expression,
			u.Reading,
			u.IsActive,
			new LearningUnitKnowledgeCollectionDto(
				u.KnowledgeCollection.Id.Value,
				u.KnowledgeCollection.Code,
				u.KnowledgeCollection.Name,
				u.KnowledgeCollection.CollectionType.ToString()));

	internal static LearningUnitMeaningDto MapMeaning(
		Domain.Entities.Knowledge.KnowledgeMeaning.KnowledgeMeaning m, bool isUsed) =>
		new(m.Id.Value, m.SortOrder, m.Meaning, m.JapaneseMeaning, m.Description, isUsed);
}