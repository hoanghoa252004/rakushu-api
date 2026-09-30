using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.GetJapaneseConjugationForms;

public sealed record GetJapaneseConjugationFormsQuery : IRequest<Result<IReadOnlyCollection<JapaneseConjugationFormDto>>>;
