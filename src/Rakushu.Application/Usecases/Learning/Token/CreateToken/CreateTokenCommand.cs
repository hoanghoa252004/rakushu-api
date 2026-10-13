using Entity = Rakushu.Domain.Entities.Material.Token.Token;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Token.CreateToken;

public sealed record CreateTokenCommand(
	Guid bunsetsuId,
	string surface,
	string lemma,
	string reading,
	Guid japanesePartOfSpeechId,
	Guid universalPartOfSpeechId,
	Guid dependencyRelationshipId,
	int startIndex,
	int endIndex,
	int sequence
) : IRequest<Result<Guid>>;
