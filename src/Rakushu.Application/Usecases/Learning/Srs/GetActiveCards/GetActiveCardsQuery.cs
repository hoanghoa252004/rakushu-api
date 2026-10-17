using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.GetActiveCards;

public sealed record GetActiveCardsQuery : IRequest<Result<IReadOnlyList<SrsCardDto>>>;
