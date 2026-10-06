using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.CreateProficiencyLevel;

internal sealed class CreateProficiencyLevelHandler : IRequestHandler<CreateProficiencyLevelCommand, Result<Guid>>
{
	private readonly IUnitOfWork _unitOfWork;

	public CreateProficiencyLevelHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateProficiencyLevelCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			return Result.Success(Guid.NewGuid());
		}, cancellationToken);
	}
}