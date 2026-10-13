using Entity = Rakushu.Domain.Entities.Knowledge.ContentCategory.ContentCategory;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.DeleteContentCategory;

public sealed record DeleteContentCategoryCommand(Guid ContentCategoryId) : IRequest<Result>;
