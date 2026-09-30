using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech.DeleteJapanesePartOfSpeech;

public sealed record DeleteJapanesePartOfSpeechCommand(Guid JapanesePartOfSpeechId) : IRequest<Result>;
