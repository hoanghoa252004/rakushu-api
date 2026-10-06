using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Create;

public sealed record CreateJapaneseConjugationFormCommand(
	string Code,
	string Name,
	string JapaneseName,
	string? Description) : IRequest<Result<Guid>>;
