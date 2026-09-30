using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;

namespace Rakushu.Application.Usecases.ProficiencyFramework.UpdateProficiencyFramework;

internal sealed class UpdateProficiencyFrameworkHandler : IRequestHandler<UpdateProficiencyFrameworkCommand, Result>
{
	private readonly IProficiencyFrameworkRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateProficiencyFrameworkHandler(IProficiencyFrameworkRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateProficiencyFrameworkCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var framework = await _repository.GetByIdAsync(ProficiencyFrameworkId.From(request.ProficiencyFrameworkId), cancellationToken);
			if (framework is null)
				return Result.Failure(ProficiencyFrameworkErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var updateResult = framework.Update(
				request.code,
				request.name,
				request.isActive,
				now,
				request.description);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}