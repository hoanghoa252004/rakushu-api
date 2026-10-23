using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetKnowledgeMeaningById;

public sealed record GetKnowledgeMeaningByIdQuery(
	Guid KnowledgeMeaningId
	) : IRequest<Result<KnowledgeMeaningDto>>;

public sealed record KnowledgeMeaningDto(
	Guid Id,
	Guid KnowledgeUnitId,
	int SortOrder,
	string Meaning,
	string JapaneseMeaning,
	string? Description
	);

internal sealed class GetKnowledgeMeaningByIdHandler : IRequestHandler<GetKnowledgeMeaningByIdQuery, Result<KnowledgeMeaningDto>>
{
	private readonly IKnowledgeMeaningRepository _repository;
	private readonly IKnowledgeUnitRepository _unitRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetKnowledgeMeaningByIdHandler(
		IKnowledgeMeaningRepository repository,
		IKnowledgeUnitRepository unitRepository,
		ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_unitRepository = unitRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<KnowledgeMeaningDto>> Handle(GetKnowledgeMeaningByIdQuery request, CancellationToken cancellationToken)
	{
		var meaning = await _repository.GetByIdAsync(KnowledgeMeaningId.From(request.KnowledgeMeaningId), cancellationToken);

		if (meaning is null)
			return Result.Failure<KnowledgeMeaningDto>(KnowledgeMeaningErrors.NotFound);

		// Learners can only see meanings of active knowledge units
		if (_currentUserContext.Role != RoleCodes.LinguisticCurator)
		{
			var unit = await _unitRepository.GetByIdAsync(meaning.KnowledgeUnitId, cancellationToken);
			if (unit is null || !unit.IsActive)
				return Result.Failure<KnowledgeMeaningDto>(KnowledgeMeaningErrors.NotFound);
		}

		return Result.Success(new KnowledgeMeaningDto(
			meaning.Id.Value,
			meaning.KnowledgeUnitId.Value,
			meaning.SortOrder,
			meaning.Meaning,
			meaning.JapaneseMeaning,
			meaning.Description));
	}
}
