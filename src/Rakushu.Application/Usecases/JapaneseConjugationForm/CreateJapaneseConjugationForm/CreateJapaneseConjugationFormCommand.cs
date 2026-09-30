using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm.CreateJapaneseConjugationForm;

public sealed record CreateJapaneseConjugationFormCommand(
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result<Guid>>;
