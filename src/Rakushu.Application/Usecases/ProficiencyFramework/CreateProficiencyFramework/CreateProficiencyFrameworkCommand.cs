using Entity = Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.CreateProficiencyFramework;

public sealed record CreateProficiencyFrameworkCommand(
	string code,
	string name,
	string? description,
	bool isActive
) : IRequest<Result<Guid>>;
