using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevels;

internal sealed class GetProficiencyLevelsHandler : IRequestHandler<GetProficiencyLevelsQuery, Result<IReadOnlyCollection<ProficiencyLevelDto>>>
{

	public GetProficiencyLevelsHandler()
	{
	}

	public async Task<Result<IReadOnlyCollection<ProficiencyLevelDto>>> Handle(GetProficiencyLevelsQuery request, CancellationToken cancellationToken)
	{
		
		return Result.Success<IReadOnlyCollection<ProficiencyLevelDto>>(Array.Empty<ProficiencyLevelDto>());
	}
}