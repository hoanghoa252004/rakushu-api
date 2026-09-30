using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token.Token;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Token.GetTokens;

public sealed record GetTokensQuery : IRequest<Result<IReadOnlyCollection<TokenDto>>>;
