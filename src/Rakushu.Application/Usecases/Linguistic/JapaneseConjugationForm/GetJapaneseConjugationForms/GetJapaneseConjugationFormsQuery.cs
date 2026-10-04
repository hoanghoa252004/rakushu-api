using Entity = Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetJapaneseConjugationForms;

public sealed record GetJapaneseConjugationFormsQuery : IRequest<Result<IReadOnlyCollection<JapaneseConjugationFormDto>>>;
