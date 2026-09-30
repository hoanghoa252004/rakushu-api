using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.UpdateJapaneseConjugationForm;

public sealed record UpdateJapaneseConjugationFormCommand(
	Guid JapaneseConjugationFormId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
