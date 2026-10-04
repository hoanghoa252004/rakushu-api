using Entity = Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetJapanesePartOfSpeechById;

public sealed record GetJapanesePartOfSpeechByIdQuery(Guid JapanesePartOfSpeechId) : IRequest<Result<JapanesePartOfSpeechDto>>;
