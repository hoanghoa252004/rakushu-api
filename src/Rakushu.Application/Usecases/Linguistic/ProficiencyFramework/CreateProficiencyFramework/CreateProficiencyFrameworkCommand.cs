using Entity = Rakushu.Domain.Entities.Linguistic.ProficiencyFramework.ProficiencyFramework;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.CreateProficiencyFramework;

public sealed record CreateProficiencyFrameworkCommand(
	string code,
	string name,
	string? description,
	bool isActive
) : IRequest<Result<Guid>>;
