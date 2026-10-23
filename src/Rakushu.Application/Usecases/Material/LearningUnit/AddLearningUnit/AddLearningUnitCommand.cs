using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.LearningUnit.AddLearningUnit;

public sealed record AddLearningUnitCommand(
	Guid VideoId,
	Guid SegmentId,
	Guid KnowledgeMeaningId,
	int StartTokenIndex,
	int EndTokenIndex
) : IRequest<Result<Guid>>;
