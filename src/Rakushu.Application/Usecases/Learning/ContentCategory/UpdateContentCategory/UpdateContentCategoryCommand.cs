using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.UpdateContentCategory;

public sealed record UpdateContentCategoryCommand(
	Guid ContentCategoryId,
	string Name,
	string JapaneseName,
	int DisplayOrder,
	string ThemeColor,
	bool IsActive,
	string? Description = null
) : IRequest<Result>;
