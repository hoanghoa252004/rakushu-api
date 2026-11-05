using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.GetShadowingMaterialRequests;

public sealed record GetShadowingMaterialRequestsQuery(
	int PageNumber,
	int PageSize,
	Guid? VideoId,
	Guid? UserId,
	ShadowingMaterialRequestStatus? Status)
	: IRequest<Result<PaginatedList<ShadowingMaterialRequestDto>>>;

internal sealed class GetShadowingMaterialRequestsValidator : AbstractValidator<GetShadowingMaterialRequestsQuery>
{
	public GetShadowingMaterialRequestsValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than 0");
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");
	}
}

internal sealed class GetShadowingMaterialRequestsHandler(
	IShadowingMaterialRequestRepository repository,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingMaterialRequestsQuery, Result<PaginatedList<ShadowingMaterialRequestDto>>>
{
	public async Task<Result<PaginatedList<ShadowingMaterialRequestDto>>> Handle(
		GetShadowingMaterialRequestsQuery request, CancellationToken cancellationToken)
	{
		var isCurator = currentUser.Role == RoleCodes.LinguisticCurator;

		// Learners can only see their own requests.
		UserId? learnerId;
		if (isCurator)
			learnerId = request.UserId.HasValue ? UserId.From(request.UserId.Value) : null;
		else
		{
			if (currentUser.UserId is null)
				return Result.Failure<PaginatedList<ShadowingMaterialRequestDto>>(ShadowingMaterialRequestErrors.Forbidden);
			learnerId = currentUser.UserId;
		}

		var (items, total) = await repository.GetPagedAsync(
			request.PageNumber,
			request.PageSize,
			request.VideoId.HasValue ? VideoId.From(request.VideoId.Value) : null,
			learnerId,
			request.Status,
			cancellationToken);

		return Result.Success(PaginatedList<ShadowingMaterialRequestDto>.Create(
			items.Select(ShadowingMaterialRequestDto.FromEntity).ToList(),
			total,
			request.PageNumber,
			request.PageSize));
	}
}
