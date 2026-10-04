using Entity = Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.DeleteJapanesePartOfSpeech;

public sealed record DeleteJapanesePartOfSpeechCommand(Guid JapanesePartOfSpeechId) : IRequest<Result>;
