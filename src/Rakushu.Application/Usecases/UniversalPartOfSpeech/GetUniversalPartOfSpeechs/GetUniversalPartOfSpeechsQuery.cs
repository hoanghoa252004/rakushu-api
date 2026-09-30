using Entity = Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.UniversalPartOfSpeech.GetUniversalPartOfSpeechs;

public sealed record GetUniversalPartOfSpeechsQuery : IRequest<Result<IReadOnlyCollection<UniversalPartOfSpeechDto>>>;
