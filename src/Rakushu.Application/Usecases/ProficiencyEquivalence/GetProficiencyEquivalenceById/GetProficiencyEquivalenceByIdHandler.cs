using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.GetProficiencyEquivalenceById;

internal sealed class GetProficiencyEquivalenceByIdHandler : IRequestHandler<GetProficiencyEquivalenceByIdQuery, Result<ProficiencyEquivalenceDto>>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;

	public GetProficiencyEquivalenceByIdHandler(IProficiencyFrameworkRepository frameworkRepository)
	{
		_frameworkRepository = frameworkRepository;
	}

	public async Task<Result<ProficiencyEquivalenceDto>> Handle(GetProficiencyEquivalenceByIdQuery request, CancellationToken cancellationToken)
	{
		var eqId = ProficiencyEquivalenceId.From(request.ProficiencyEquivalenceId);
		var frameworks = await _frameworkRepository.GetAllAsync(cancellationToken);
		foreach (var framework in frameworks)
		{
			foreach (var level in framework.ProficiencyLevels)
			{
				var eq = level.SourceLevelProficiencyEquivalences.FirstOrDefault(e => e.Id == eqId);
				if (eq is not null)
				{
					return Result.Success(ProficiencyEquivalenceDto.FromEntity(eq));
				}
			}
		}

		return Result.Failure<ProficiencyEquivalenceDto>(ProficiencyEquivalenceErrors.NotFound);
	}
}