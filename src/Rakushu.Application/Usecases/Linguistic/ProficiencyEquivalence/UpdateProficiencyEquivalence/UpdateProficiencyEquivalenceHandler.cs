using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.UpdateProficiencyEquivalence;

internal sealed class UpdateProficiencyEquivalenceHandler : IRequestHandler<UpdateProficiencyEquivalenceCommand, Result>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateProficiencyEquivalenceHandler(IProficiencyFrameworkRepository frameworkRepository, IUnitOfWork unitOfWork)
	{
		_frameworkRepository = frameworkRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateProficiencyEquivalenceCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var eqId = ProficiencyEquivalenceId.From(request.ProficiencyEquivalenceId);
			var sourceLevelId = ProficiencyLevelId.From(request.sourceLevelId);
			var framework = await _frameworkRepository.GetByLevelIdAsync(sourceLevelId, cancellationToken);
			if (framework is null)
				return Result.Failure(ProficiencyLevelErrors.NotFound);

			var level = framework.ProficiencyLevels.FirstOrDefault(l => l.Id == sourceLevelId);
			if (level is null)
				return Result.Failure(ProficiencyLevelErrors.NotFound);

			var eq = level.SourceLevelProficiencyEquivalences.FirstOrDefault(e => e.Id == eqId);
			if (eq is null)
				return Result.Failure(ProficiencyEquivalenceErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			return eq.Update(
				sourceLevelId,
				ProficiencyLevelId.From(request.targetLevelId),
				request.type,
				now,
				request.note,
				request.reference);
		}, cancellationToken);
	}
}