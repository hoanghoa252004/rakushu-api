using Entity = Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

public sealed record GetUniversalPartOfSpeechByIdQuery(Guid UniversalPartOfSpeechId) : IRequest<Result<UniversalPartOfSpeechDto>>;
