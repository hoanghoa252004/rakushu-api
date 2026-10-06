using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

internal sealed class GetProficiencyLevelByIdHandler : IRequestHandler<GetProficiencyLevelByIdQuery, Result<ProficiencyLevelDto>>
{

	public GetProficiencyLevelByIdHandler()
	{
		
	}

	public async Task<Result<ProficiencyLevelDto>> Handle(GetProficiencyLevelByIdQuery request, CancellationToken cancellationToken)
	{
		return Result.Success(new ProficiencyLevelDto());
	}
}