using Entity = Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.DeleteJapaneseConjugationForm;

public sealed record DeleteJapaneseConjugationFormCommand(Guid JapaneseConjugationFormId) : IRequest<Result>;
