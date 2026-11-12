using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitCurationContext;

public sealed record GetLearningUnitCurationContextQuery(
	Guid LearningUnitId
) : IRequest<Result<LearningUnitCurationContextDto>>;
