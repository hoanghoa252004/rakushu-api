using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.GetKnowledgeCollectionById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.GetKnowledgeCollections;

public sealed record GetKnowledgeCollectionsQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? SearchTerm = null,
	string? Active = null,
	string? Type = null,
	string? Scope = null
) : IRequest<Result<PaginatedList<KnowledgeCollectionDto>>>;

public sealed class GetKnowledgeCollectionsValidator : AbstractValidator<GetKnowledgeCollectionsQuery>
{
	public GetKnowledgeCollectionsValidator()
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

internal sealed class GetKnowledgeCollectionsHandler : IRequestHandler<GetKnowledgeCollectionsQuery, Result<PaginatedList<KnowledgeCollectionDto>>>
{
	private readonly IKnowledgeCollectionRepository _repository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetKnowledgeCollectionsHandler(IKnowledgeCollectionRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<PaginatedList<KnowledgeCollectionDto>>> Handle(GetKnowledgeCollectionsQuery request, CancellationToken cancellationToken)
	{
		var isCurator = _currentUserContext.Role == RoleCodes.LinguisticCurator;

		bool? active = null;
		if (!string.IsNullOrWhiteSpace(request.Active))
		{
			if (!bool.TryParse(request.Active.Trim(), out var parsedActive))
				return Result.Failure<PaginatedList<KnowledgeCollectionDto>>(KnowledgeCollectionErrors.InvalidFilterActive);
			active = parsedActive;
		}

		KnowledgeCollectionType? type = null;
		if (!string.IsNullOrWhiteSpace(request.Type))
		{
			if (!Enum.TryParse<KnowledgeCollectionType>(request.Type.Trim(), true, out var parsedType)
				|| !Enum.IsDefined(parsedType))
				return Result.Failure<PaginatedList<KnowledgeCollectionDto>>(KnowledgeCollectionErrors.InvalidFilterType);
			type = parsedType;
		}

		KnowledgeCollectionScope? scope = null;
		if (!string.IsNullOrWhiteSpace(request.Scope))
		{
			if (!Enum.TryParse<KnowledgeCollectionScope>(request.Scope.Trim(), true, out var parsedScope)
				|| !Enum.IsDefined(parsedScope))
				return Result.Failure<PaginatedList<KnowledgeCollectionDto>>(KnowledgeCollectionErrors.InvalidFilterType);
			scope = parsedScope;
		}

		var query = (await _repository.GetAllAsync(cancellationToken)).AsEnumerable();

		// Learners can only see active records
		if (!isCurator)
			query = query.Where(c => c.IsActive);
		else if (active.HasValue)
			query = query.Where(c => c.IsActive == active.Value);

		if (type.HasValue)
			query = query.Where(c => c.CollectionType == type.Value);

		if (scope.HasValue)
			query = query.Where(c => c.Scope == scope.Value);

		if (!string.IsNullOrWhiteSpace(request.SearchTerm))
		{
			var term = request.SearchTerm.Trim();
			query = query.Where(c =>
				c.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
				c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
				(c.Description != null && c.Description.Contains(term, StringComparison.OrdinalIgnoreCase)));
		}

		var list = query.ToList();

		var items = list
			.OrderBy(c => c.Code)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(c => new KnowledgeCollectionDto(
				c.Id.Value,
				c.Code,
				c.Name,
				c.Description,
				c.CollectionType,
				c.Scope,
				c.IsActive,
				c.CreatedAt,
				c.UpdatedAt))
			.ToList();

		return Result.Success(PaginatedList<KnowledgeCollectionDto>.Create(items, list.Count, request.PageNumber, request.PageSize));
	}
}
