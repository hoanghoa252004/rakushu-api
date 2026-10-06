using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.UpdateProficiencyLevel;

internal sealed class UpdateProficiencyLevelHandler : IRequestHandler<UpdateProficiencyLevelCommand, Result>
{
	private readonly IUnitOfWork _unitOfWork;

	public UpdateProficiencyLevelHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateProficiencyLevelCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			return Result.Success();
		}, cancellationToken);
	}
}