using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.GetKnowledgeUnitById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.GetKnowledgeUnits;

public sealed record GetKnowledgeUnitsQuery(
	int PageNumber = 1,
	int PageSize = 10,
	Guid? KnowledgeCollectionId = null,
	string? SearchTerm = null,
	string? Active = null,
	string? Type = null
) : IRequest<Result<PaginatedList<KnowledgeUnitDto>>>;

public sealed class GetKnowledgeUnitsValidator : AbstractValidator<GetKnowledgeUnitsQuery>
{
	public GetKnowledgeUnitsValidator()
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

internal sealed class GetKnowledgeUnitsHandler : IRequestHandler<GetKnowledgeUnitsQuery, Result<PaginatedList<KnowledgeUnitDto>>>
{
	private readonly IKnowledgeUnitRepository _repository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetKnowledgeUnitsHandler(IKnowledgeUnitRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<PaginatedList<KnowledgeUnitDto>>> Handle(GetKnowledgeUnitsQuery request, CancellationToken cancellationToken)
	{
		var isCurator = _currentUserContext.Role == RoleCodes.LinguisticCurator;

		bool? active = null;
		if (!string.IsNullOrWhiteSpace(request.Active))
		{
			if (!bool.TryParse(request.Active.Trim(), out var parsedActive))
				return Result.Failure<PaginatedList<KnowledgeUnitDto>>(KnowledgeUnitErrors.InvalidFilterActive);
			active = parsedActive;
		}

		KnowledgeUnitType? type = null;
		if (!string.IsNullOrWhiteSpace(request.Type))
		{
			if (!Enum.TryParse<KnowledgeUnitType>(request.Type.Trim(), true, out var parsedType)
				|| !Enum.IsDefined(parsedType))
				return Result.Failure<PaginatedList<KnowledgeUnitDto>>(KnowledgeUnitErrors.InvalidFilterType);
			type = parsedType;
		}

		var query = (await _repository.GetAllAsync(cancellationToken)).AsEnumerable();

		// Learners can only see active records
		if (!isCurator)
			query = query.Where(u => u.IsActive);
		else if (active.HasValue)
			query = query.Where(u => u.IsActive == active.Value);

		if (request.KnowledgeCollectionId.HasValue)
			query = query.Where(u => u.KnowledgeCollectionId.Value == request.KnowledgeCollectionId.Value);

		if (type.HasValue)
			query = query.Where(u => u.KnowledgeType == type.Value);

		if (!string.IsNullOrWhiteSpace(request.SearchTerm))
		{
			var term = request.SearchTerm.Trim();
			query = query.Where(u =>
				u.Expression.Contains(term, StringComparison.OrdinalIgnoreCase) ||
				u.Reading.Contains(term, StringComparison.OrdinalIgnoreCase));
		}

		var list = query.ToList();

		var items = list
			.OrderBy(u => u.Expression)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(u => new KnowledgeUnitDto(
				u.Id.Value,
				u.KnowledgeCollectionId.Value,
				u.ProficiencyLevelId.Value,
				u.KnowledgeType,
				u.Expression,
				u.Reading,
				u.AudioKey,
				u.SourceType,
				u.IsActive,
				u.CreatedAt,
				u.UpdatedAt))
			.ToList();

		return Result.Success(PaginatedList<KnowledgeUnitDto>.Create(items, list.Count, request.PageNumber, request.PageSize));
	}
}
