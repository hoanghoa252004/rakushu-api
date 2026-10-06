using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.DeleteProficiencyLevel;

public sealed record DeleteProficiencyLevelCommand(Guid ProficiencyLevelId) : IRequest<Result>;
