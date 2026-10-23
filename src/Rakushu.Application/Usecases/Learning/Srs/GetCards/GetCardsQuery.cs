using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.GetCards;

public sealed record GetCardsQuery : IRequest<Result<IReadOnlyList<SrsCardDto>>>;
