using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevels;

internal sealed class GetProficiencyLevelsHandler : IRequestHandler<GetProficiencyLevelsQuery, Result<IReadOnlyCollection<ProficiencyLevelDto>>>
{
	private readonly IProficiencyLevelRepository _proficiencyLevelRepository;

	public GetProficiencyLevelsHandler(IProficiencyLevelRepository proficiencyLevelRepository)
	{
		_proficiencyLevelRepository = proficiencyLevelRepository;
	}

	public async Task<Result<IReadOnlyCollection<ProficiencyLevelDto>>> Handle(GetProficiencyLevelsQuery request, CancellationToken cancellationToken)
	{
		var levels = await _proficiencyLevelRepository.GetAllAsync();

		var levelDtos = levels.Select(level => new ProficiencyLevelDto(
			level.Id.Value,
			level.Code,
			level.Name,
			level.JapaneseName,
			level.SortOrder,
			level.IsActive,
			level.Description,
			level.CreatedAt,
			level.UpdatedAt
		)).ToList();

		return Result.Success<IReadOnlyCollection<ProficiencyLevelDto>>(levelDtos);
	}
}