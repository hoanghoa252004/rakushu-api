using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.DeleteProficiencyFramework;

internal sealed class DeleteProficiencyFrameworkHandler : IRequestHandler<DeleteProficiencyFrameworkCommand, Result>
{
	private readonly IProficiencyFrameworkRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteProficiencyFrameworkHandler(IProficiencyFrameworkRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteProficiencyFrameworkCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var framework = await _repository.GetByIdAsync(ProficiencyFrameworkId.From(request.ProficiencyFrameworkId), cancellationToken);
			if (framework is null)
				return Result.Failure(ProficiencyFrameworkErrors.NotFound);

			_repository.Delete(framework);
			return Result.Success();
		}, cancellationToken);
	}
}