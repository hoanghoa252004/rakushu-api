using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.GetJapaneseConjugationFormById;

public sealed record GetJapaneseConjugationFormByIdQuery(Guid JapaneseConjugationFormId) : IRequest<Result<JapaneseConjugationFormDto>>;
