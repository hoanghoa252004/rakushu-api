using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.UpdateContentCategory;

public sealed record UpdateContentCategoryCommand(
	Guid ContentCategoryId,
	string slug,
	string code,
	string name,
	string? description,
	Guid? parentId,
	int level,
	int displayOrder,
	bool isActive
) : IRequest<Result>;
