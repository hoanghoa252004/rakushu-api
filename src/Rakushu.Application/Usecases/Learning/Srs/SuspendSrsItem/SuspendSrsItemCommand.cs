using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SuspendSrsItem;

public sealed record SuspendSrsItemCommand(Guid Id) : IRequest<Result<SrsItemDto>>;
