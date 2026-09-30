using Entity = Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.UniversalPartOfSpeech.UpdateUniversalPartOfSpeech;

public sealed record UpdateUniversalPartOfSpeechCommand(
	Guid UniversalPartOfSpeechId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
