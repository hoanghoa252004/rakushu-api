using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.LearningUnit.RemoveLearningUnit;

public sealed record RemoveLearningUnitCommand(
	Guid VideoId,
	Guid SegmentId,
	Guid LearningUnitId
) : IRequest<Result>;
