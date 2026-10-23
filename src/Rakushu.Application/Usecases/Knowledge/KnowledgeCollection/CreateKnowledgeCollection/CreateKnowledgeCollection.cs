using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.CreateKnowledgeCollection;

public sealed record CreateKnowledgeCollectionCommand(
	string Code,
	string Name,
	KnowledgeCollectionType CollectionType,
	KnowledgeCollectionScope Scope,
	bool IsActive,
	string? Description = null
) : IRequest<Result<Guid>>;

internal sealed class CreateKnowledgeCollectionValidator : AbstractValidator<CreateKnowledgeCollectionCommand>
{
	public CreateKnowledgeCollectionValidator()
	{
		RuleFor(x => x.Code)
			.NotEmpty().WithMessage("Knowledge collection code is required")
			.MaximumLength(30).WithMessage("Knowledge collection code must not exceed 30 characters");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Knowledge collection name is required")
			.MaximumLength(100).WithMessage("Knowledge collection name must not exceed 100 characters");

		RuleFor(x => x.CollectionType).IsInEnum();
		RuleFor(x => x.Scope).IsInEnum();
	}
}

internal sealed class CreateKnowledgeCollectionHandler : IRequestHandler<CreateKnowledgeCollectionCommand, Result<Guid>>
{
	private readonly IKnowledgeCollectionRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public CreateKnowledgeCollectionHandler(IKnowledgeCollectionRepository repository, IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result<Guid>> Handle(CreateKnowledgeCollectionCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var existing = await _repository.GetByCodeAsync(request.Code, cancellationToken);

			if (existing is not null)
				return Result.Failure<Guid>(KnowledgeCollectionErrors.DuplicateCode);

			var result = Domain.Entities.Knowledge.KnowledgeCollection.KnowledgeCollection.Create(
				request.Code,
				request.Name,
				request.CollectionType,
				request.Scope,
				request.IsActive,
				_systemClock.UtcNow,
				request.Description);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			_repository.Add(result.Value);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}
