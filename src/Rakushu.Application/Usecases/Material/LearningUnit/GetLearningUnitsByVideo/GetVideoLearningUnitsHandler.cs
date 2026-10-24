using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitsByVideo;



internal sealed class GetVideoLearningUnitsHandler(
	ILearningUnitRepository repository,
	IVideoRepository videoRepository)
	: IRequestHandler<GetVideoLearningUnitsQuery, Result<PaginatedList<LearningUnitSummaryDto>>>
{
	public async Task<Result<PaginatedList<LearningUnitSummaryDto>>> Handle(
		GetVideoLearningUnitsQuery request,
		CancellationToken cancellationToken)
	{
		KnowledgeUnitType? type = null;
		if (!string.IsNullOrWhiteSpace(request.Type))
		{
			if (!Enum.TryParse<KnowledgeUnitType>(request.Type.Trim(), true, out var parsed)
				|| !Enum.IsDefined(parsed))
			{
				return Result.Failure<PaginatedList<LearningUnitSummaryDto>>(
					Domain.Common.Errors.Error.Validation(
						"LEARNING_UNIT.INVALID_TYPE",
						"Type must be one of: Word, Collocation, Grammar, Terminology, ProperNoun."));
			}
			type = parsed;
		}

		var videoId = VideoId.From(request.VideoId);
		var exists = (await videoRepository.FindAsync(v => v.Id == videoId, cancellationToken)).Any();
		if (!exists)
			return Result.Failure<PaginatedList<LearningUnitSummaryDto>>(VideoErrors.NotFound);

		var (items, total) = await repository.GetPagedByVideoIdAsync(
			videoId, type, request.PageNumber, request.PageSize, cancellationToken);

		var dtos = items.Select(l => new LearningUnitSummaryDto(
			l.Id.Value,
			l.TranscriptSegmentId.Value,
			l.StartTokenIndex,
			l.EndTokenIndex,
			l.Confidence,
			l.IsReviewed,
			l.DetectionMethod.ToString(),
			LearningUnitMapper2.MapUnit(l.KnowledgeMeaning.KnowledgeUnit),
			LearningUnitMapper2.MapMeaning(l.KnowledgeMeaning, true))).ToList();

		return Result.Success(PaginatedList<LearningUnitSummaryDto>.Create(dtos, total, request.PageNumber, request.PageSize));
	}
}
