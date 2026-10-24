using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SuspendSrsItem;

public sealed record SuspendSrsItemCommand(Guid ItemId) : IRequest<Result<SrsItemDto>>;
