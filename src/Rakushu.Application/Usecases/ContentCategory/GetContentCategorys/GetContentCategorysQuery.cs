using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.ContentCategory.GetContentCategorys;

public sealed record GetContentCategorysQuery : IRequest<Result<IReadOnlyCollection<ContentCategoryDto>>>;
