using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetKnowledgeMeaningById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetKnowledgeMeanings;

public sealed record GetKnowledgeMeaningsQuery(
	Guid KnowledgeUnitId,
	string? SearchTerm = null
) : IRequest<Result<IReadOnlyList<KnowledgeMeaningDto>>>;

internal sealed class GetKnowledgeMeaningsHandler : IRequestHandler<GetKnowledgeMeaningsQuery, Result<IReadOnlyList<KnowledgeMeaningDto>>>
{
	private readonly IKnowledgeMeaningRepository _repository;
	private readonly IKnowledgeUnitRepository _unitRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetKnowledgeMeaningsHandler(
		IKnowledgeMeaningRepository repository,
		IKnowledgeUnitRepository unitRepository,
		ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_unitRepository = unitRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<IReadOnlyList<KnowledgeMeaningDto>>> Handle(GetKnowledgeMeaningsQuery request, CancellationToken cancellationToken)
	{
		var unitId = KnowledgeUnitId.From(request.KnowledgeUnitId);

		var unit = await _unitRepository.GetByIdAsync(unitId, cancellationToken);

		// Learners can only see meanings of active knowledge units
		if (unit is null || (!unit.IsActive && _currentUserContext.Role != RoleCodes.LinguisticCurator))
			return Result.Failure<IReadOnlyList<KnowledgeMeaningDto>>(KnowledgeMeaningErrors.UnitNotFound);

		var meanings = (await _repository.GetByKnowledgeUnitIdAsync(unitId, cancellationToken)).AsEnumerable();

		if (!string.IsNullOrWhiteSpace(request.SearchTerm))
		{
			var term = request.SearchTerm.Trim();
			meanings = meanings.Where(m =>
				m.Meaning.Contains(term, StringComparison.OrdinalIgnoreCase) ||
				m.JapaneseMeaning.Contains(term, StringComparison.OrdinalIgnoreCase));
		}

		IReadOnlyList<KnowledgeMeaningDto> items = meanings
			.Select(m => new KnowledgeMeaningDto(
				m.Id.Value,
				m.KnowledgeUnitId.Value,
				m.SortOrder,
				m.Meaning,
				m.JapaneseMeaning,
				m.Description))
			.ToList();

		return Result.Success(items);
	}
}
