using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.UpdateProficiencyLevel;

internal sealed class UpdateProficiencyLevelHandler : IRequestHandler<UpdateProficiencyLevelCommand, Result>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateProficiencyLevelHandler(IProficiencyFrameworkRepository frameworkRepository, IUnitOfWork unitOfWork)
	{
		_frameworkRepository = frameworkRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateProficiencyLevelCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var levelId = ProficiencyLevelId.From(request.ProficiencyLevelId);
			var framework = await _frameworkRepository.GetByIdAsync(ProficiencyFrameworkId.From(request.frameworkId), cancellationToken)
				?? await _frameworkRepository.GetByLevelIdAsync(levelId, cancellationToken);

			if (framework is null)
				return Result.Failure(ProficiencyFrameworkErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			return framework.UpdateLevel(levelId, request.code, request.name, request.sortOrder, now, request.description);
		}, cancellationToken);
	}
}