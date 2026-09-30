using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.UpdateProficiencyFramework;

public sealed record UpdateProficiencyFrameworkCommand(
	Guid ProficiencyFrameworkId,
	string code,
	string name,
	string? description,
	bool isActive
) : IRequest<Result>;
