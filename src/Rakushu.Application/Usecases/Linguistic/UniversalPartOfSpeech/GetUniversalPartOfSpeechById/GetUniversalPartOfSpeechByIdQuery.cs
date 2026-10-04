using Entity = Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

public sealed record GetUniversalPartOfSpeechByIdQuery(Guid UniversalPartOfSpeechId) : IRequest<Result<UniversalPartOfSpeechDto>>;
