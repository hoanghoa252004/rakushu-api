using MediatR;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

internal sealed class GetProficiencyLevelByIdHandler : IRequestHandler<GetProficiencyLevelByIdQuery, Result<ProficiencyLevelDto>>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;

	public GetProficiencyLevelByIdHandler(IProficiencyFrameworkRepository frameworkRepository)
	{
		_frameworkRepository = frameworkRepository;
	}

	public async Task<Result<ProficiencyLevelDto>> Handle(GetProficiencyLevelByIdQuery request, CancellationToken cancellationToken)
	{
		var levelId = ProficiencyLevelId.From(request.ProficiencyLevelId);
		var framework = await _frameworkRepository.GetByLevelIdAsync(levelId, cancellationToken);
		if (framework is null)
			return Result.Failure<ProficiencyLevelDto>(ProficiencyLevelErrors.NotFound);

		var level = framework.ProficiencyLevels.FirstOrDefault(l => l.Id == levelId);
		if (level is null)
			return Result.Failure<ProficiencyLevelDto>(ProficiencyLevelErrors.NotFound);

		return Result.Success(ProficiencyLevelDto.FromEntity(level));
	}
}