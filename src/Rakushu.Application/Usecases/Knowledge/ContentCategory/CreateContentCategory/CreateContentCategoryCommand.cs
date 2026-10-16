using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Knowledge.ContentCategory.CreateContentCategory;

public sealed record CreateContentCategoryCommand(
	string Slug,
	string Code,
	string Name,
	string JapaneseName,
	int DisplayOrder,
	string ThemeColor,
	bool IsActive,
	string? Description = null
) : IRequest<Result<Guid>>;
