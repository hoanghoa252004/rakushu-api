using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech.JapanesePartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech.GetJapanesePartOfSpeechs;

public sealed record GetJapanesePartOfSpeechsQuery : IRequest<Result<IReadOnlyCollection<JapanesePartOfSpeechDto>>>;
