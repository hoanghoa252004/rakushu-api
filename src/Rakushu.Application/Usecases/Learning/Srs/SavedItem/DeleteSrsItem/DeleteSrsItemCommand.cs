using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.DeleteSrsItem;

public sealed record DeleteSrsItemCommand(Guid Id) : IRequest<Result>;
