using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.DeleteSrsItem;

public sealed record DeleteSrsItemCommand(Guid Id) : IRequest<Result>;
