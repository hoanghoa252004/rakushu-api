using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.ContentCategory.DeleteContentCategory;

public sealed record DeleteContentCategoryCommand(Guid ContentCategoryId) : IRequest<Result>;
