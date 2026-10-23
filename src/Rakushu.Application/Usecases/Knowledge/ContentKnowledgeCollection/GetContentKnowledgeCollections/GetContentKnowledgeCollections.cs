using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.GetContentKnowledgeCollectionById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.GetContentKnowledgeCollections;

public sealed record GetContentKnowledgeCollectionsQuery(
	int PageNumber = 1,
	int PageSize = 10,
	Guid? ContentCategoryId = null,
	string? SearchTerm = null,
	string? Active = null,
	string? Type = null
) : IRequest<Result<PaginatedList<ContentKnowledgeCollectionDto>>>;

public sealed class GetContentKnowledgeCollectionsValidator : AbstractValidator<GetContentKnowledgeCollectionsQuery>
{
	public GetContentKnowledgeCollectionsValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Page number must be greater than or equal to 1");

		RuleFor(x => x.PageSize)
			.GreaterThanOrEqualTo(1)
			.LessThanOrEqualTo(100)
			.WithMessage("Page size must be between 1 and 100");
	}
}

internal sealed class GetContentKnowledgeCollectionsHandler : IRequestHandler<GetContentKnowledgeCollectionsQuery, Result<PaginatedList<ContentKnowledgeCollectionDto>>>
{
	private readonly IContentKnowledgeCollectionRepository _repository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetContentKnowledgeCollectionsHandler(IContentKnowledgeCollectionRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<PaginatedList<ContentKnowledgeCollectionDto>>> Handle(GetContentKnowledgeCollectionsQuery request, CancellationToken cancellationToken)
	{
		var isCurator = _currentUserContext.Role == RoleCodes.LinguisticCurator;

		bool? active = null;
		if (!string.IsNullOrWhiteSpace(request.Active))
		{
			if (!bool.TryParse(request.Active.Trim(), out var parsedActive))
				return Result.Failure<PaginatedList<ContentKnowledgeCollectionDto>>(ContentKnowledgeCollectionErrors.InvalidFilterActive);
			active = parsedActive;
		}

		KnowledgeCollectionType? type = null;
		if (!string.IsNullOrWhiteSpace(request.Type))
		{
			if (!Enum.TryParse<KnowledgeCollectionType>(request.Type.Trim(), true, out var parsedType)
				|| !Enum.IsDefined(parsedType))
				return Result.Failure<PaginatedList<ContentKnowledgeCollectionDto>>(ContentKnowledgeCollectionErrors.InvalidFilterType);
			type = parsedType;
		}

		var query = (await _repository.GetAllAsync(cancellationToken)).AsEnumerable();

		// Learners can only see active records
		if (!isCurator)
			query = query.Where(x => x.IsActive);
		else if (active.HasValue)
			query = query.Where(x => x.IsActive == active.Value);

		if (request.ContentCategoryId.HasValue)
			query = query.Where(x => x.ContentCategoryId.Value == request.ContentCategoryId.Value);

		if (type.HasValue)
			query = query.Where(x => x.KnowledgeCollection.CollectionType == type.Value);

		if (!string.IsNullOrWhiteSpace(request.SearchTerm))
		{
			var term = request.SearchTerm.Trim();
			query = query.Where(x =>
				x.KnowledgeCollection.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
				x.KnowledgeCollection.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
				x.ContentCategory.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
		}

		var list = query.ToList();

		var items = list
			.OrderBy(x => x.ContentCategory.DisplayOrder)
			.ThenBy(x => x.Priority)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(x => new ContentKnowledgeCollectionDto(
				x.Id.Value,
				x.ContentCategoryId.Value,
				x.ContentCategory.Name,
				x.KnowledgeCollectionId.Value,
				x.KnowledgeCollection.Code,
				x.KnowledgeCollection.Name,
				x.KnowledgeCollection.CollectionType,
				x.Priority,
				x.IsActive))
			.ToList();

		return Result.Success(PaginatedList<ContentKnowledgeCollectionDto>.Create(items, list.Count, request.PageNumber, request.PageSize));
	}
}
