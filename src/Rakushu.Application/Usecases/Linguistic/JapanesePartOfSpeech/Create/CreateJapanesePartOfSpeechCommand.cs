using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Create;

public sealed record CreateJapanesePartOfSpeechCommand(
	string Code,
	string Name,
	string JapaneseName,
	string? Description) : IRequest<Result<Guid>>;
