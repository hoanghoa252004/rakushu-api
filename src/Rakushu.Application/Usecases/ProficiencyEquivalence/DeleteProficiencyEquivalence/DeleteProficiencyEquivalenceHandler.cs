using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.DeleteProficiencyEquivalence;

internal sealed class DeleteProficiencyEquivalenceHandler : IRequestHandler<DeleteProficiencyEquivalenceCommand, Result>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteProficiencyEquivalenceHandler(IProficiencyFrameworkRepository frameworkRepository, IUnitOfWork unitOfWork)
	{
		_frameworkRepository = frameworkRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteProficiencyEquivalenceCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var eqId = ProficiencyEquivalenceId.From(request.ProficiencyEquivalenceId);
			var frameworks = await _frameworkRepository.GetAllAsync(cancellationToken);
			foreach (var framework in frameworks)
			{
				foreach (var level in framework.ProficiencyLevels)
				{
					var eq = level.SourceLevelProficiencyEquivalences.FirstOrDefault(e => e.Id == eqId);
					if (eq is not null)
					{
						return level.RemoveEquivalence(eqId);
					}
				}
			}

			return Result.Failure(ProficiencyEquivalenceErrors.NotFound);
		}, cancellationToken);
	}
}