using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.GetProficiencyEquivalences;

internal sealed class GetProficiencyEquivalencesHandler : IRequestHandler<GetProficiencyEquivalencesQuery, Result<IReadOnlyCollection<ProficiencyEquivalenceDto>>>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;

	public GetProficiencyEquivalencesHandler(IProficiencyFrameworkRepository frameworkRepository)
	{
		_frameworkRepository = frameworkRepository;
	}

	public async Task<Result<IReadOnlyCollection<ProficiencyEquivalenceDto>>> Handle(GetProficiencyEquivalencesQuery request, CancellationToken cancellationToken)
	{
		var frameworks = await _frameworkRepository.GetAllAsync(cancellationToken);
		var items = frameworks
			.SelectMany(f => f.ProficiencyLevels)
			.SelectMany(l => l.SourceLevelProficiencyEquivalences)
			.Select(ProficiencyEquivalenceDto.FromEntity)
			.ToArray();

		return Result.Success<IReadOnlyCollection<ProficiencyEquivalenceDto>>(items);
	}
}