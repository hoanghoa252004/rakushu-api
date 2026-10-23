using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.UpdateKnowledgeCollection;

public sealed record UpdateKnowledgeCollectionCommand(
	Guid KnowledgeCollectionId,
	string Name,
	KnowledgeCollectionType CollectionType,
	KnowledgeCollectionScope Scope,
	bool IsActive,
	string? Description = null
) : IRequest<Result>;

internal sealed class UpdateKnowledgeCollectionValidator : AbstractValidator<UpdateKnowledgeCollectionCommand>
{
	public UpdateKnowledgeCollectionValidator()
	{
		RuleFor(x => x.KnowledgeCollectionId).NotEmpty().WithMessage("Knowledge collection ID is required");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Knowledge collection name is required")
			.MaximumLength(100).WithMessage("Knowledge collection name must not exceed 100 characters");

		RuleFor(x => x.CollectionType).IsInEnum();
		RuleFor(x => x.Scope).IsInEnum();
	}
}

internal sealed class UpdateKnowledgeCollectionHandler : IRequestHandler<UpdateKnowledgeCollectionCommand, Result>
{
	private readonly IKnowledgeCollectionRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public UpdateKnowledgeCollectionHandler(IKnowledgeCollectionRepository repository, IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(UpdateKnowledgeCollectionCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var collection = await _repository.GetByIdAsync(KnowledgeCollectionId.From(request.KnowledgeCollectionId), cancellationToken);

			if (collection is null)
				return Result.Failure(KnowledgeCollectionErrors.NotFound);

			var updateResult = collection.Update(
				request.Name,
				request.CollectionType,
				request.Scope,
				request.IsActive,
				_systemClock.UtcNow,
				request.Description);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}
