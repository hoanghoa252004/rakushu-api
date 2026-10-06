using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.CreateProficiencyLevel;

public sealed record CreateProficiencyLevelCommand(
	Guid frameworkId,
	string code,
	string name,
	int sortOrder,
	string? description
) : IRequest<Result<Guid>>;
