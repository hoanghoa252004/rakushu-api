using Entity = Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.UpdateJapaneseConjugationForm;

public sealed record UpdateJapaneseConjugationFormCommand(
	Guid JapaneseConjugationFormId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
