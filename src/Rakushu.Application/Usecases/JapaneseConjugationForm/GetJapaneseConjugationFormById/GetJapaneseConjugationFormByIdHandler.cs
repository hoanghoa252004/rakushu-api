using MediatR;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.GetJapaneseConjugationFormById;

internal sealed class GetJapaneseConjugationFormByIdHandler : IRequestHandler<GetJapaneseConjugationFormByIdQuery, Result<JapaneseConjugationFormDto>>
{
	private readonly IJapaneseConjugationFormRepository _repository;

	public GetJapaneseConjugationFormByIdHandler(IJapaneseConjugationFormRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<JapaneseConjugationFormDto>> Handle(GetJapaneseConjugationFormByIdQuery request, CancellationToken cancellationToken)
	{
		var form = await _repository.GetByIdAsync(JapaneseConjugationFormId.From(request.JapaneseConjugationFormId), cancellationToken);
		if (form is null)
			return Result.Failure<JapaneseConjugationFormDto>(Error.NotFound("JAPANESE_CONJUGATION_FORM.NOT_FOUND", "Japanese conjugation form not found."));

		return Result.Success(JapaneseConjugationFormDto.FromEntity(form));
	}
}