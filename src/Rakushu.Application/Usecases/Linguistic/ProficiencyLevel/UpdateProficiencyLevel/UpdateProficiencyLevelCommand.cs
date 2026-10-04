using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyLevel;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.UpdateProficiencyLevel;

public sealed record UpdateProficiencyLevelCommand(
	Guid ProficiencyLevelId,
	Guid frameworkId,
	string code,
	string name,
	int sortOrder,
	string? description
) : IRequest<Result>;
