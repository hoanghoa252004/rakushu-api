using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ProficiencyFramework;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;

namespace Rakushu.Application.Usecases.ProficiencyEquivalence.CreateProficiencyEquivalence;

internal sealed class CreateProficiencyEquivalenceHandler : IRequestHandler<CreateProficiencyEquivalenceCommand, Result<Guid>>
{
	private readonly IProficiencyFrameworkRepository _frameworkRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateProficiencyEquivalenceHandler(IProficiencyFrameworkRepository frameworkRepository, IUnitOfWork unitOfWork)
	{
		_frameworkRepository = frameworkRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateProficiencyEquivalenceCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var sourceLevelId = ProficiencyLevelId.From(request.sourceLevelId);
			var framework = await _frameworkRepository.GetByLevelIdAsync(sourceLevelId, cancellationToken);
			if (framework is null)
				return Result.Failure<Guid>(ProficiencyLevelErrors.NotFound);

			var level = framework.ProficiencyLevels.FirstOrDefault(l => l.Id == sourceLevelId);
			if (level is null)
				return Result.Failure<Guid>(ProficiencyLevelErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var result = level.AddEquivalence(
				ProficiencyLevelId.From(request.targetLevelId),
				request.type,
				now,
				now,
				request.note,
				request.reference);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}