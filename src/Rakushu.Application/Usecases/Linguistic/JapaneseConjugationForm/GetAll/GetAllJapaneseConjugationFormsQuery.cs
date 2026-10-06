using MediatR;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetAll;

public sealed record GetAllJapaneseConjugationFormsQuery : IRequest<List<JapaneseConjugationFormDto>>;

public sealed class GetAllJapaneseConjugationFormsHandler : IRequestHandler<GetAllJapaneseConjugationFormsQuery, List<JapaneseConjugationFormDto>>
{
	private readonly IJapaneseConjugationFormRepository _repository;

	public GetAllJapaneseConjugationFormsHandler(IJapaneseConjugationFormRepository repository)
	{
		_repository = repository;
	}

	public async Task<List<JapaneseConjugationFormDto>> Handle(GetAllJapaneseConjugationFormsQuery query, CancellationToken cancellationToken)
	{
		var entities = await _repository.GetAllAsync(cancellationToken);
		return entities.Select(e => new JapaneseConjugationFormDto
		{
			Id = e.Id.Value,
			Code = e.Code,
			Name = e.Name,
			JapaneseName = e.JapaneseName,
			Description = e.Description
		}).ToList();
	}
}
