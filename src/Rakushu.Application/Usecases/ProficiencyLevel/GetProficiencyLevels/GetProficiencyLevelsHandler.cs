using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyLevel.GetProficiencyLevels;

internal sealed class GetProficiencyLevelsHandler : IRequestHandler<GetProficiencyLevelsQuery, Result<IReadOnlyCollection<ProficiencyLevelDto>>>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;

	public GetProficiencyLevelsHandler(IProficiencyFrameworkRepository frameworkRepository)
	{
		_frameworkRepository = frameworkRepository;
	}

	public async Task<Result<IReadOnlyCollection<ProficiencyLevelDto>>> Handle(GetProficiencyLevelsQuery request, CancellationToken cancellationToken)
	{
		var frameworks = await _frameworkRepository.GetAllAsync(cancellationToken);
		var levels = frameworks
			.SelectMany(f => f.ProficiencyLevels)
			.Select(ProficiencyLevelDto.FromEntity)
			.ToArray();

		return Result.Success<IReadOnlyCollection<ProficiencyLevelDto>>(levels);
	}
}