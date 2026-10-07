using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.UpdateProficiencyLevel;

public sealed record UpdateProficiencyLevelCommand(
	Guid ProficiencyLevelId,
	string Name,
	string JapaneseName,
	int SortOrder,
	bool IsActive,
	string? Description = null
) : IRequest<Result>;
