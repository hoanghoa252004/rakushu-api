using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.ResumeSrsItem;

public sealed record ResumeSrsItemCommand(Guid Id) : IRequest<Result<SrsItemDto>>;
