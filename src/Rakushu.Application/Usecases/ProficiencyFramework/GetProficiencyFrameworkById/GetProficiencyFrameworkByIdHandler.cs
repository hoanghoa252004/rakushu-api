using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.GetProficiencyFrameworkById;

internal sealed class GetProficiencyFrameworkByIdHandler : IRequestHandler<GetProficiencyFrameworkByIdQuery, Result<ProficiencyFrameworkDto>>
{
	private readonly IProficiencyFrameworkRepository _repository;

	public GetProficiencyFrameworkByIdHandler(IProficiencyFrameworkRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<ProficiencyFrameworkDto>> Handle(GetProficiencyFrameworkByIdQuery request, CancellationToken cancellationToken)
	{
		var framework = await _repository.GetByIdAsync(ProficiencyFrameworkId.From(request.ProficiencyFrameworkId), cancellationToken);
		if (framework is null)
			return Result.Failure<ProficiencyFrameworkDto>(ProficiencyFrameworkErrors.NotFound);

		return Result.Success(ProficiencyFrameworkDto.FromEntity(framework));
	}
}