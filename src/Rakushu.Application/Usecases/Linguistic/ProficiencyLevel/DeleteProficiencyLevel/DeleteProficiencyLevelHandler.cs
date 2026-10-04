using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.DeleteProficiencyLevel;

internal sealed class DeleteProficiencyLevelHandler : IRequestHandler<DeleteProficiencyLevelCommand, Result>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteProficiencyLevelHandler(IProficiencyFrameworkRepository frameworkRepository, IUnitOfWork unitOfWork)
	{
		_frameworkRepository = frameworkRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteProficiencyLevelCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var levelId = ProficiencyLevelId.From(request.ProficiencyLevelId);
			var framework = await _frameworkRepository.GetByLevelIdAsync(levelId, cancellationToken);
			if (framework is null)
				return Result.Failure(ProficiencyLevelErrors.NotFound);

			return framework.RemoveLevel(levelId);
		}, cancellationToken);
	}
}