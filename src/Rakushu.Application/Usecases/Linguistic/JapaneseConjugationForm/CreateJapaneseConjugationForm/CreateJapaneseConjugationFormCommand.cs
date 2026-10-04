using Entity = Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.CreateJapaneseConjugationForm;

public sealed record CreateJapaneseConjugationFormCommand(
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result<Guid>>;
