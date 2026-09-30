using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech.GetJapanesePartOfSpeechById;

public sealed record GetJapanesePartOfSpeechByIdQuery(Guid JapanesePartOfSpeechId) : IRequest<Result<JapanesePartOfSpeechDto>>;
