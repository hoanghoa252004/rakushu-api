using Entity = Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech.UniversalPartOfSpeech;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.CreateUniversalPartOfSpeech;

public sealed record CreateUniversalPartOfSpeechCommand(
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result<Guid>>;
