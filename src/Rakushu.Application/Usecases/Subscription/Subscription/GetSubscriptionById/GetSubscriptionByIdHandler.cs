using MediatR;
using Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptionById;

internal sealed class GetSubscriptionByIdHandler : IRequestHandler<GetSubscriptionByIdQuery, Result<SubscriptionDetailDto>>
{
	private readonly ISubscriptionRepository _subscriptionRepository;

	public GetSubscriptionByIdHandler(ISubscriptionRepository subscriptionRepository)
	{
		_subscriptionRepository = subscriptionRepository;
	}

	public async Task<Result<SubscriptionDetailDto>> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
	{
		var subscriptionId = SubscriptionId.From(request.SubscriptionId);

		var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);

		if (subscription == null)
		{
			return Result.Failure<SubscriptionDetailDto>(SubscriptionErrors.NotFound);
		}

		var subscriptionDetailDto = new SubscriptionDetailDto
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
			);

		return Result.Success(subscriptionDetailDto);
	}
}


