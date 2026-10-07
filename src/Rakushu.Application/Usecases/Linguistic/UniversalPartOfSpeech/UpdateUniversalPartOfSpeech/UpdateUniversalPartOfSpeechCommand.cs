using Entity = Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.UpdateUniversalPartOfSpeech;

public sealed record UpdateUniversalPartOfSpeechCommand(
	Guid UniversalPartOfSpeechId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
