using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.GetItems;

public sealed record GetItemsQuery : IRequest<Result<IReadOnlyList<SrsItemDto>>>;
