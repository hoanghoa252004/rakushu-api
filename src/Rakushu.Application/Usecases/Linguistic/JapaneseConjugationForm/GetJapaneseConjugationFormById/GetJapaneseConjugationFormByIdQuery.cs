using Entity = Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.GetJapaneseConjugationFormById;

public sealed record GetJapaneseConjugationFormByIdQuery(Guid JapaneseConjugationFormId) : IRequest<Result<JapaneseConjugationFormDto>>;
