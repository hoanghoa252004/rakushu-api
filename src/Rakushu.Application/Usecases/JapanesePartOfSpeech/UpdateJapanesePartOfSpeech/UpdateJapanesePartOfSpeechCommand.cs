using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech.UpdateJapanesePartOfSpeech;

public sealed record UpdateJapanesePartOfSpeechCommand(
	Guid JapanesePartOfSpeechId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
