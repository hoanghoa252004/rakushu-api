using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token.Token;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;

namespace Rakushu.Application.Usecases.Token.DeleteToken;

public sealed record DeleteTokenCommand(Guid TokenId) : IRequest<Result>;
