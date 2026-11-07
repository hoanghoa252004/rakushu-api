using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.SaveSrsItem;

public sealed record SaveSrsItemCommand(Guid MeaningId) : IRequest<Result<Guid>>;
