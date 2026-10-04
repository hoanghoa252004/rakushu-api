using MediatR;
using Rakushu.Application.Usecases.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.GetProficiencyFrameworks;

internal sealed class GetProficiencyFrameworksHandler : IRequestHandler<GetProficiencyFrameworksQuery, Result<IReadOnlyCollection<ProficiencyFrameworkDto>>>
{
	private readonly IProficiencyFrameworkRepository _repository;

	public GetProficiencyFrameworksHandler(IProficiencyFrameworkRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<IReadOnlyCollection<ProficiencyFrameworkDto>>> Handle(GetProficiencyFrameworksQuery request, CancellationToken cancellationToken)
	{
		var list = await _repository.GetAllAsync(cancellationToken);
		return Result.Success<IReadOnlyCollection<ProficiencyFrameworkDto>>(list.Select(ProficiencyFrameworkDto.FromEntity).ToArray());
	}
}