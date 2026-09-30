using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.DeleteJapaneseConjugationForm;

public sealed record DeleteJapaneseConjugationFormCommand(Guid JapaneseConjugationFormId) : IRequest<Result>;
