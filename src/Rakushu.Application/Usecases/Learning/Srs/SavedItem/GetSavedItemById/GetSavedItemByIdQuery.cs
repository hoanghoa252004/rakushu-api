using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSavedItemById;

public sealed record GetSavedItemByIdQuery(Guid Id) : IRequest<Result<SavedItemDetailDto>>;
