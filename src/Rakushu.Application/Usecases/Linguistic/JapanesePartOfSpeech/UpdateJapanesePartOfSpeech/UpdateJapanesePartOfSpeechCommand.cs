using Entity = Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.UpdateJapanesePartOfSpeech;

public sealed record UpdateJapanesePartOfSpeechCommand(
	Guid JapanesePartOfSpeechId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
