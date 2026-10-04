using Entity = Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechs;

public sealed record GetUniversalPartOfSpeechsQuery : IRequest<Result<IReadOnlyCollection<UniversalPartOfSpeechDto>>>;
