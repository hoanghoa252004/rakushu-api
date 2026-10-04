using Entity = Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.CreateJapanesePartOfSpeech;

public sealed record CreateJapanesePartOfSpeechCommand(
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result<Guid>>;
