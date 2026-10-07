using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

internal sealed class GetProficiencyLevelByIdHandler : IRequestHandler<GetProficiencyLevelByIdQuery, Result<ProficiencyLevelDto>>
{
	private readonly IProficiencyLevelRepository _proficiencyLevelRepository;

	public GetProficiencyLevelByIdHandler(IProficiencyLevelRepository proficiencyLevelRepository	  )
	{
		_proficiencyLevelRepository = proficiencyLevelRepository;
	}

	public async Task<Result<ProficiencyLevelDto>> Handle(GetProficiencyLevelByIdQuery request, CancellationToken cancellationToken)
	{
		var levelId = ProficiencyLevelId.From(request.ProficiencyLevelId);

		var level = await _proficiencyLevelRepository.GetByIdAsync(levelId);

		if (level is null)
		{
			return Result.Failure<ProficiencyLevelDto>(ProficiencyLevelErrors.NotFound);
		}

		return Result.Success(new ProficiencyLevelDto(
			level.Id.Value,
			level.Code,
			level.Name,
			level.JapaneseName,
			level.SortOrder,
			level.IsActive,
			level.Description,
			level.CreatedAt,
			level.UpdatedAt
		));
	}
}