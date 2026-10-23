using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.CreateKnowledgeUnit;

public sealed record CreateKnowledgeUnitCommand(
	Guid KnowledgeCollectionId,
	Guid ProficiencyLevelId,
	KnowledgeUnitType KnowledgeType,
	string Expression,
	string Reading,
	bool IsActive,
	string? AudioKey = null
) : IRequest<Result<Guid>>;

internal sealed class CreateKnowledgeUnitValidator : AbstractValidator<CreateKnowledgeUnitCommand>
{
	public CreateKnowledgeUnitValidator()
	{
		RuleFor(x => x.KnowledgeCollectionId).NotEmpty().WithMessage("Knowledge collection ID is required");
		RuleFor(x => x.ProficiencyLevelId).NotEmpty().WithMessage("Proficiency level ID is required");
		RuleFor(x => x.KnowledgeType).IsInEnum();

		RuleFor(x => x.Expression)
			.NotEmpty().WithMessage("Expression is required")
			.MaximumLength(100).WithMessage("Expression must not exceed 100 characters");

		RuleFor(x => x.Reading)
			.NotEmpty().WithMessage("Reading is required")
			.MaximumLength(100).WithMessage("Reading must not exceed 100 characters");

		RuleFor(x => x.AudioKey)
			.MaximumLength(100).WithMessage("Audio key must not exceed 100 characters");
	}
}

internal sealed class CreateKnowledgeUnitHandler : IRequestHandler<CreateKnowledgeUnitCommand, Result<Guid>>
{
	private readonly IKnowledgeUnitRepository _repository;
	private readonly IKnowledgeCollectionRepository _collectionRepository;
	private readonly IProficiencyLevelRepository _proficiencyLevelRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public CreateKnowledgeUnitHandler(
		IKnowledgeUnitRepository repository,
		IKnowledgeCollectionRepository collectionRepository,
		IProficiencyLevelRepository proficiencyLevelRepository,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_repository = repository;
		_collectionRepository = collectionRepository;
		_proficiencyLevelRepository = proficiencyLevelRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result<Guid>> Handle(CreateKnowledgeUnitCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var collectionId = KnowledgeCollectionId.From(request.KnowledgeCollectionId);
			var levelId = ProficiencyLevelId.From(request.ProficiencyLevelId);

			if (await _collectionRepository.GetByIdAsync(collectionId, cancellationToken) is null)
				return Result.Failure<Guid>(KnowledgeUnitErrors.CollectionNotFound);

			if (await _proficiencyLevelRepository.GetByIdAsync(levelId, cancellationToken) is null)
				return Result.Failure<Guid>(KnowledgeUnitErrors.ProficiencyLevelNotFound);

			var result = Domain.Entities.Knowledge.KnowledgeUnit.KnowledgeUnit.Create(
				collectionId,
				levelId,
				request.KnowledgeType,
				request.Expression,
				request.Reading,
				KnowledgeUnitSourceType.Defined,
				request.IsActive,
				_systemClock.UtcNow,
				request.AudioKey);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			_repository.Add(result.Value);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
