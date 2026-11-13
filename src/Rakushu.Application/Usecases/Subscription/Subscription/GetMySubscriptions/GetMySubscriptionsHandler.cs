using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Application.Usecases.Subscription.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptionById;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Subscriptions.Plan;
using Rakushu.Domain.Entities.Subscriptions.Subscription;

namespace Rakushu.Application.Usecases.Subscription.Subscription.GetMySubscriptions;

internal sealed class GetMySubscriptionsHandler : IRequestHandler<GetMySubscriptionsQuery, Result<PaginatedList<SubscriptionDetailDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISubscriptionRepository _subscriptionRepository;

	public GetMySubscriptionsHandler(
		ICurrentUserContext currentUserContext,
		ISubscriptionRepository subscriptionRepository)
	{
		_currentUserContext = currentUserContext;
		_subscriptionRepository = subscriptionRepository;
	}

	public async Task<Result<PaginatedList<SubscriptionDetailDto>>> Handle(GetMySubscriptionsQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;

		if (userId == null)
		{
			return Result.Failure<PaginatedList<SubscriptionDetailDto>>(SubscriptionErrors.InvalidUserId);
		}

		SubscriptionStatus? status = null;
		if (!string.IsNullOrWhiteSpace(request.Status))
		{
			if (!Enum.TryParse<SubscriptionStatus>(request.Status, true, out var parsedStatus))
				return Result.Failure<PaginatedList<SubscriptionDetailDto>>(SubscriptionErrors.InvalidStatus);

			status = parsedStatus;
		}

		var (page, totalCount) = await _subscriptionRepository.GetPagedByUserAsync(
			userId,
			request.PlanId.HasValue ? PlanId.From(request.PlanId.Value) : null,
			status,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var items = page
			.Select(subscription => new SubscriptionDetailDto
			(
				subscription.Id.Value,
				subscription.Status.ToString(),
				subscription.StartAt,
				subscription.EndAt,
				new PlanDto
				(
					subscription.Plan.Id.Value,
					subscription.Plan.Code,
					subscription.Plan.Name,
					subscription.Plan.JapaneseName,
					subscription.Plan.Price,
					subscription.Plan.Currency.ToString(),
					subscription.Plan.BillingCycle.ToString(),
					subscription.Plan.IsActive,
					subscription.Plan.CreatedAt,
					subscription.Plan.UpdatedAt,
					subscription.Plan.Entitlements.Select(e => new EntitlementDto
					(
						e.Id.Value,
						e.IsEnabled,
						e.LimitUnit.ToString(),
						e.LimitValue,
						e.LimitPeriod.ToString(),
						new FeatureDto
						(
							e.Feature.Id.Value,
							e.Feature.Code,
							e.Feature.Name,
							e.Feature.IsActive,
							e.Feature.CreatedAt,
							e.Feature.UpdatedAt)
					)).ToList(),
					subscription.Plan.Description
				),
				subscription.SubscriptionUsages.Select(usage => new SubscriptionUsageDto
				(
					usage.Id.Value,
					usage.PeriodStart,
					usage.PeriodEnd,
					usage.MaxValue,
					usage.UsedValue,
					usage.IsOverLimit,
					usage.IsExpired,
					usage.IsCanceled,
					usage.CreatedAt,
					usage.UpdatedAt,
					new FeatureDto
					(
						usage.Feature.Id.Value,
						usage.Feature.Code,
						usage.Feature.Name,
						usage.Feature.IsActive,
						usage.Feature.CreatedAt,
						usage.Feature.UpdatedAt
					)
				)).ToList(),
				new UserBasicDto
				(
					subscription.User.Id.Value,
					subscription.User.FullName,
					subscription.User.Email
				),
				subscription.Payment is not null ? new PaymentBasicDto(
					subscription.Payment.Id.Value,
					subscription.Payment.Amount,
					subscription.Payment.Currency.ToString(),
					subscription.Payment.Status.ToString(),
					subscription.Payment.ExpiredAt,
					subscription.Payment.CreatedAt,
					subscription.Payment.UpdatedAt
				) : null
			)).ToList();

		var paginatedList = new PaginatedList<SubscriptionDetailDto>(
			items.ToList(),
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}


