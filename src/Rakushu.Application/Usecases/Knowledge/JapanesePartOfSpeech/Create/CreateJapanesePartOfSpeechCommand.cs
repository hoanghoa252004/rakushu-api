using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Knowledge.JapanesePartOfSpeech.Create;

public sealed record CreateJapanesePartOfSpeechCommand(
	string Code,
	string Name,
	string JapaneseName,
	string? Description) : IRequest<Result<Guid>>;
