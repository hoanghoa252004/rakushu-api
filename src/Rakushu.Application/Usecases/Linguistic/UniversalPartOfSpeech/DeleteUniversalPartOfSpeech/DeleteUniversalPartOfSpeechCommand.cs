using Entity = Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.DeleteUniversalPartOfSpeech;

public sealed record DeleteUniversalPartOfSpeechCommand(Guid UniversalPartOfSpeechId) : IRequest<Result>;
