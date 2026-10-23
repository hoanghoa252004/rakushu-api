using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.GetKnowledgeUnitById;

public sealed record GetKnowledgeUnitByIdQuery(
	Guid KnowledgeUnitId
	) : IRequest<Result<KnowledgeUnitDto>>;

public sealed record KnowledgeUnitDto(
	Guid Id,
	Guid KnowledgeCollectionId,
	Guid ProficiencyLevelId,
	KnowledgeUnitType KnowledgeType,
	string Expression,
	string Reading,
	string? AudioKey,
	KnowledgeUnitSourceType SourceType,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
	);

internal sealed class GetKnowledgeUnitByIdHandler : IRequestHandler<GetKnowledgeUnitByIdQuery, Result<KnowledgeUnitDto>>
{
	private readonly IKnowledgeUnitRepository _repository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetKnowledgeUnitByIdHandler(IKnowledgeUnitRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<KnowledgeUnitDto>> Handle(GetKnowledgeUnitByIdQuery request, CancellationToken cancellationToken)
	{
		var unit = await _repository.GetByIdAsync(KnowledgeUnitId.From(request.KnowledgeUnitId), cancellationToken);

		if (unit is null || (!unit.IsActive && _currentUserContext.Role != RoleCodes.LinguisticCurator))
			return Result.Failure<KnowledgeUnitDto>(KnowledgeUnitErrors.NotFound);

		return Result.Success(new KnowledgeUnitDto(
			unit.Id.Value,
			unit.KnowledgeCollectionId.Value,
			unit.ProficiencyLevelId.Value,
			unit.KnowledgeType,
			unit.Expression,
			unit.Reading,
			unit.AudioKey,
			unit.SourceType,
			unit.IsActive,
			unit.CreatedAt,
			unit.UpdatedAt));
	}
}
