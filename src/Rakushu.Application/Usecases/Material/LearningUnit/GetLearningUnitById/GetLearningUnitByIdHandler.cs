using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.LearningUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitById;



internal sealed class GetLearningUnitByIdHandler(ILearningUnitRepository repository)
	: IRequestHandler<GetLearningUnitByIdQuery, Result<LearningUnitDto>>
{
	public async Task<Result<LearningUnitDto>> Handle(GetLearningUnitByIdQuery request, CancellationToken cancellationToken)
	{
		var unit = await repository.GetDetailByIdAsync(LearningUnitId.From(request.LearningUnitId), cancellationToken);
		if (unit is null)
			return Result.Failure<LearningUnitDto>(LearningUnitErrors.NotFound);

		var knowledgeUnit = unit.KnowledgeMeaning.KnowledgeUnit;
		var meanings = knowledgeUnit.Meanings
			.OrderBy(m => m.SortOrder)
			.Select(m => LearningUnitMapper2.MapMeaning(m, m.Id == unit.KnowledgeMeaningId))
			.ToList();

		return Result.Success(new LearningUnitDto(
			unit.Id.Value,
			unit.TranscriptSegmentId.Value,
			unit.StartTokenIndex,
			unit.EndTokenIndex,
			unit.Confidence,
			unit.IsReviewed,
			unit.DetectionMethod.ToString(),
			LearningUnitMapper2.MapUnit(knowledgeUnit),
			LearningUnitMapper2.MapMeaning(unit.KnowledgeMeaning, true),
			meanings.Count,
			meanings));
	}
}