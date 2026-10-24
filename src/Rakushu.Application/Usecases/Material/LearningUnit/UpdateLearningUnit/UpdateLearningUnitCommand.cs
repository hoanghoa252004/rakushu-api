using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.LearningUnit.UpdateLearningUnit;

public sealed record UpdateLearningUnitCommand(
	Guid VideoId,
	Guid SegmentId,
	Guid LearningUnitId,
	Guid KnowledgeMeaningId,
	int StartTokenIndex,
	int EndTokenIndex
) : IRequest<Result>;
