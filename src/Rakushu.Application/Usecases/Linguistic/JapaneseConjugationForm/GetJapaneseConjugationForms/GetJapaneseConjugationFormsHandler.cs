using MediatR;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetJapaneseConjugationForms;

internal sealed class GetJapaneseConjugationFormsHandler : IRequestHandler<GetJapaneseConjugationFormsQuery, Result<IReadOnlyCollection<JapaneseConjugationFormDto>>>
{
	private readonly IJapaneseConjugationFormRepository _repository;

	public GetJapaneseConjugationFormsHandler(IJapaneseConjugationFormRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<IReadOnlyCollection<JapaneseConjugationFormDto>>> Handle(GetJapaneseConjugationFormsQuery request, CancellationToken cancellationToken)
	{
		var list = await _repository.GetAllAsync(cancellationToken);
		return Result.Success<IReadOnlyCollection<JapaneseConjugationFormDto>>(list.Select(JapaneseConjugationFormDto.FromEntity).ToArray());
	}
}