using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Application.Usecases.Subscription.GetSubscriptionById;

public sealed class GetSubscriptionByIdHandler : IRequestHandler<GetSubscriptionByIdQuery, Result<SubscriptionDetailDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISubscriptionQuery _subscriptionQuery;

	public GetSubscriptionByIdHandler(
		ICurrentUserContext currentUserContext,
		ISubscriptionQuery subscriptionQuery)
	{
		_currentUserContext = currentUserContext;
		_subscriptionQuery = subscriptionQuery;
	}

	public async Task<Result<SubscriptionDetailDto>> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure<SubscriptionDetailDto>(UserError.NotFound);
		}

		var subscription = await _subscriptionQuery.GetByIdAsync(
			SubscriptionId.From(request.SubscriptionId),
			cancellationToken);

		if (subscription == null)
		{
			return Result.Failure<SubscriptionDetailDto>(SubscriptionErrors.NotFound);
		}

		var isAdmin = _currentUserContext.Role == DefaultSystemRoles.SystemAdministrator.ToString();
		if (!isAdmin && subscription.UserId != userId.Value)
		{
			return Result.Failure<SubscriptionDetailDto>(UserError.UnauthorizedResourceAccess);
		}

		return Result.Success(subscription);
	}
}
