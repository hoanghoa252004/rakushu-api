using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;

internal sealed class GetSubscriptionsHandler : IRequestHandler<GetSubscriptionsQuery, Result<PaginatedList<SubscriptionDto>>>
{
	private readonly ISubscriptionRepository _subscriptionRepository;

	public GetSubscriptionsHandler(
		ISubscriptionRepository subscriptionRepository)
	{
		_subscriptionRepository = subscriptionRepository;
	}

	public async Task<Result<PaginatedList<SubscriptionDto>>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
	{
		// Validate status if provided
		if (!string.IsNullOrWhiteSpace(request.Status) && !Enum.TryParse<SubscriptionStatus>(request.Status, true, out _))
		{
			return Result.Failure<PaginatedList<SubscriptionDto>>(SubscriptionErrors.InvalidStatus);
		}

		var list = await _subscriptionRepository.GetAllAsync(cancellationToken);

		if (request.UserId.HasValue)
		{
			list = list.Where(p => p.UserId.Value == request.UserId);
		}

		if (request.PlanId.HasValue)
		{
			list = list.Where(p => p.PlanId.Value == request.PlanId);
		}

		if (!string.IsNullOrWhiteSpace(request.Status))
		{
			list = list.Where(p => p.Status.ToString().ToLower() == request.Status.ToLower());
		}

		// Calculate total count before paging
		var totalCount = list.Count();

		// Apply paging
		var items = list
			.OrderByDescending(c => c.StartAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(subscription => new SubscriptionDto(
				subscription.Id.Value,
				subscription.Status.ToString(),
				subscription.StartAt,
				subscription.EndAt,
				subscription.Payment is not null ? new PaymentBasicDto(
					subscription.PaymentId!.Value,
					subscription.Payment.Amount,
					subscription.Payment.Currency.ToString(),
					subscription.Payment.Status.ToString(),
					subscription.Payment.ExpiredAt,
					subscription.Payment.CreatedAt,
					subscription.Payment.UpdatedAt
				) : null,
				new UserBasicDto(
					subscription.UserId.Value,
					subscription.User.FullName,
					subscription.User.Email
				),
				new PlanBasicDto(
					subscription.PlanId.Value,
					subscription.Plan.Code,
					subscription.Plan.Name,
					subscription.Plan.JapaneseName,
					subscription.Plan.Price,
					subscription.Plan.Currency.ToString(),
					subscription.Plan.BillingCycle.ToString(),
					subscription.Plan.IsActive,
					subscription.Plan.Description
				)
			)).ToList();

		var paginatedList = new PaginatedList<SubscriptionDto>(
			items,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}

