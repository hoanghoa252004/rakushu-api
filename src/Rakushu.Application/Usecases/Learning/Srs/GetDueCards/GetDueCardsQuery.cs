using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.GetDueCards;

public sealed record GetDueCardsQuery : IRequest<Result<IReadOnlyList<SrsCardDto>>>;
