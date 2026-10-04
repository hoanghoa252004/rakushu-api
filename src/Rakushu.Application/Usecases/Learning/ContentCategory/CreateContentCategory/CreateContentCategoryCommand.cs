using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.CreateContentCategory;

public sealed record CreateContentCategoryCommand(
	string slug,
	string code,
	string name,
	string? description,
	Guid? parentId,
	int level,
	int displayOrder,
	bool isActive
) : IRequest<Result<Guid>>;
