using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyLevel;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;

namespace Rakushu.Application.Usecases.ProficiencyLevel.CreateProficiencyLevel;

public sealed record CreateProficiencyLevelCommand(
	Guid frameworkId,
	string code,
	string name,
	int sortOrder,
	string? description
) : IRequest<Result<Guid>>;
