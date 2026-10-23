using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.CreateKnowledgeMeaning;

public sealed record CreateKnowledgeMeaningCommand(
	Guid KnowledgeUnitId,
	int SortOrder,
	string Meaning,
	string JapaneseMeaning,
	string? Description = null
) : IRequest<Result<Guid>>;

internal sealed class CreateKnowledgeMeaningValidator : AbstractValidator<CreateKnowledgeMeaningCommand>
{
	public CreateKnowledgeMeaningValidator()
	{
		RuleFor(x => x.KnowledgeUnitId).NotEmpty().WithMessage("Knowledge unit ID is required");

		RuleFor(x => x.SortOrder).GreaterThan(0).WithMessage("Sort order must be greater than 0");

		RuleFor(x => x.Meaning)
			.NotEmpty().WithMessage("Meaning is required")
			.MaximumLength(100).WithMessage("Meaning must not exceed 100 characters");

		RuleFor(x => x.JapaneseMeaning)
			.NotEmpty().WithMessage("Japanese meaning is required")
			.MaximumLength(100).WithMessage("Japanese meaning must not exceed 100 characters");
	}
}

internal sealed class CreateKnowledgeMeaningHandler : IRequestHandler<CreateKnowledgeMeaningCommand, Result<Guid>>
{
	private readonly IKnowledgeMeaningRepository _repository;
	private readonly IKnowledgeUnitRepository _unitRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateKnowledgeMeaningHandler(IKnowledgeMeaningRepository repository, IKnowledgeUnitRepository unitRepository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitRepository = unitRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateKnowledgeMeaningCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var unitId = KnowledgeUnitId.From(request.KnowledgeUnitId);

			if (await _unitRepository.GetByIdAsync(unitId, cancellationToken) is null)
				return Result.Failure<Guid>(KnowledgeMeaningErrors.UnitNotFound);

			var result = Domain.Entities.Knowledge.KnowledgeMeaning.KnowledgeMeaning.Create(
				unitId,
				request.SortOrder,
				request.Meaning,
				request.JapaneseMeaning,
				request.Description);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			_repository.Add(result.Value);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
