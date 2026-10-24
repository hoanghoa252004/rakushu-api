using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.ResumeSrsItem;

public sealed record ResumeSrsItemCommand(Guid ItemId) : IRequest<Result<SrsItemDto>>;
