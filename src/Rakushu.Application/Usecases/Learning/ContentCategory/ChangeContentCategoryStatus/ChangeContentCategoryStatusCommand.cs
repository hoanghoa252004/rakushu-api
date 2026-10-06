using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.ChangeContentCategoryStatus;

public sealed record ChangeContentCategoryStatusCommand(
	Guid ContentCategoryId,
	ContentCategoryStatus NewStatus
) : IRequest<Result>;
