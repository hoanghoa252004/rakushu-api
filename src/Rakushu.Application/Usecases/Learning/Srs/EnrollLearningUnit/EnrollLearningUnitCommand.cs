using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.EnrollLearningUnit;

public sealed record EnrollLearningUnitCommand(Guid LearningUnitId) : IRequest<Result<EnrollLearningUnitResponse>>;
