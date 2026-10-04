using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Application.Usecases.Learning.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategorys;

public sealed record GetContentCategorysQuery : IRequest<Result<IReadOnlyCollection<ContentCategoryDto>>>;
