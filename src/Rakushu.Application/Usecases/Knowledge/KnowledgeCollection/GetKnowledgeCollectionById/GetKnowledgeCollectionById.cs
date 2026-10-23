using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.GetKnowledgeCollectionById;

public sealed record GetKnowledgeCollectionByIdQuery(
	Guid KnowledgeCollectionId
	) : IRequest<Result<KnowledgeCollectionDto>>;

public sealed record KnowledgeCollectionDto(
	Guid Id,
	string Code,
	string Name,
	string? Description,
	KnowledgeCollectionType CollectionType,
	KnowledgeCollectionScope Scope,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
	);

internal sealed class GetKnowledgeCollectionByIdHandler : IRequestHandler<GetKnowledgeCollectionByIdQuery, Result<KnowledgeCollectionDto>>
{
	private readonly IKnowledgeCollectionRepository _repository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetKnowledgeCollectionByIdHandler(IKnowledgeCollectionRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<KnowledgeCollectionDto>> Handle(GetKnowledgeCollectionByIdQuery request, CancellationToken cancellationToken)
	{
		var collection = await _repository.GetByIdAsync(KnowledgeCollectionId.From(request.KnowledgeCollectionId), cancellationToken);

		if (collection is null || (!collection.IsActive && _currentUserContext.Role != RoleCodes.LinguisticCurator))
			return Result.Failure<KnowledgeCollectionDto>(KnowledgeCollectionErrors.NotFound);

		return Result.Success(new KnowledgeCollectionDto(
			collection.Id.Value,
			collection.Code,
			collection.Name,
			collection.Description,
			collection.CollectionType,
			collection.Scope,
			collection.IsActive,
			collection.CreatedAt,
			collection.UpdatedAt));
	}
}
