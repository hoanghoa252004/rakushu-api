using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SaveSrsItem;

public sealed record SaveSrsItemCommand(Guid MeaningId) : IRequest<Result<SrsItemDto>>;
