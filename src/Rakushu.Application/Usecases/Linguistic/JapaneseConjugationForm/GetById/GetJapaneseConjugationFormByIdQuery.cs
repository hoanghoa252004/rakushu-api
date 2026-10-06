using MediatR;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetById;

public sealed record GetJapaneseConjugationFormByIdQuery(Guid Id) : IRequest<JapaneseConjugationFormDto?>;

public sealed class GetJapaneseConjugationFormByIdHandler : IRequestHandler<GetJapaneseConjugationFormByIdQuery, JapaneseConjugationFormDto?>
{
	private readonly IJapaneseConjugationFormRepository _repository;

	public GetJapaneseConjugationFormByIdHandler(IJapaneseConjugationFormRepository repository)
	{
		_repository = repository;
	}

	public async Task<JapaneseConjugationFormDto?> Handle(GetJapaneseConjugationFormByIdQuery query, CancellationToken cancellationToken)
	{
		var id = JapaneseConjugationFormId.From(query.Id);
		var entity = await _repository.GetByIdAsync(id, cancellationToken);
		if (entity == null) return null;

		return new JapaneseConjugationFormDto
		{
			Id = entity.Id.Value,
			Code = entity.Code,
			Name = entity.Name,
			JapaneseName = entity.JapaneseName,
			Description = entity.Description
		};
	}
}
