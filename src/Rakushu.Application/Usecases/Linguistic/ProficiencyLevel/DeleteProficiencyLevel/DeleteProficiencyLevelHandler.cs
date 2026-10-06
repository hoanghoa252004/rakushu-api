using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.DeleteProficiencyLevel;

internal sealed class DeleteProficiencyLevelHandler : IRequestHandler<DeleteProficiencyLevelCommand, Result>
{
	private readonly IUnitOfWork _unitOfWork;

	public DeleteProficiencyLevelHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteProficiencyLevelCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			return Result.Success();
		}, cancellationToken);
	}
}