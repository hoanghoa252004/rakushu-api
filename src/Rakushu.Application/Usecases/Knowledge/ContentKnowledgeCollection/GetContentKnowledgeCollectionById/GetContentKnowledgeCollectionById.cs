using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.GetContentKnowledgeCollectionById;

public sealed record GetContentKnowledgeCollectionByIdQuery(
	Guid ContentKnowledgeCollectionId
	) : IRequest<Result<ContentKnowledgeCollectionDto>>;

public sealed record ContentKnowledgeCollectionDto(
	Guid Id,
	Guid ContentCategoryId,
	string ContentCategoryName,
	Guid KnowledgeCollectionId,
	string KnowledgeCollectionCode,
	string KnowledgeCollectionName,
	KnowledgeCollectionType KnowledgeCollectionType,
	int Priority,
	bool IsActive
	);

internal sealed class GetContentKnowledgeCollectionByIdHandler : IRequestHandler<GetContentKnowledgeCollectionByIdQuery, Result<ContentKnowledgeCollectionDto>>
{
	private readonly IContentKnowledgeCollectionRepository _repository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetContentKnowledgeCollectionByIdHandler(IContentKnowledgeCollectionRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<ContentKnowledgeCollectionDto>> Handle(GetContentKnowledgeCollectionByIdQuery request, CancellationToken cancellationToken)
	{
		var item = await _repository.GetByIdAsync(ContentKnowledgeCollectionId.From(request.ContentKnowledgeCollectionId), cancellationToken);

		if (item is null || (!item.IsActive && _currentUserContext.Role != RoleCodes.LinguisticCurator))
			return Result.Failure<ContentKnowledgeCollectionDto>(ContentKnowledgeCollectionErrors.NotFound);

		return Result.Success(new ContentKnowledgeCollectionDto(
			item.Id.Value,
			item.ContentCategoryId.Value,
			item.ContentCategory.Name,
			item.KnowledgeCollectionId.Value,
			item.KnowledgeCollection.Code,
			item.KnowledgeCollection.Name,
			item.KnowledgeCollection.CollectionType,
			item.Priority,
			item.IsActive));
	}
}
