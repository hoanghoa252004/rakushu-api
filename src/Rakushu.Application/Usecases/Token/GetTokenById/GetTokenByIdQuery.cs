using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token.Token;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Token.GetTokenById;

public sealed record GetTokenByIdQuery(Guid TokenId) : IRequest<Result<TokenDto>>;
