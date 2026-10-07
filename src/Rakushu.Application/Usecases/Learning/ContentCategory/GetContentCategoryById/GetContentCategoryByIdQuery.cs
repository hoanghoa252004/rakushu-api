using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;

public sealed record GetContentCategoryByIdQuery(
	Guid ContentCategoryId
	) : IRequest<Result<ContentCategoryDto>>;

public sealed record ContentCategoryDto(
	Guid Id,
	string Slug,
	string Code,
	string Name,
	string JapaneseName,
	string? Description,
	int DisplayOrder,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
	);