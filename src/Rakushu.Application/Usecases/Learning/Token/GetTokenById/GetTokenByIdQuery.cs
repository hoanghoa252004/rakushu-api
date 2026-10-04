using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token.Token;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Learning.Token;

namespace Rakushu.Application.Usecases.Learning.Token.GetTokenById;

public sealed record GetTokenByIdQuery(Guid TokenId) : IRequest<Result<TokenDto>>;
