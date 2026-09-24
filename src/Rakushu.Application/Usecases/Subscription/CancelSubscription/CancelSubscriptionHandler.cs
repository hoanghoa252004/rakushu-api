using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.Subscription;

namespace Rakushu.Application.Usecases.Subscription.CancelSubscription;

public sealed class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUserRepository _userRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CancelSubscriptionHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId == null)
		{
			return Result.Failure(UserError.NotFound);
		}

		var subscriptionId = SubscriptionId.From(request.SubscriptionId);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var isAdmin = _currentUserContext.Role == DefaultSystemRoles.SystemAdministrator.ToString();
			var user = isAdmin
				? await _userRepository.GetBySubscriptionIdAsync(subscriptionId, cancellationToken)
				: await _userRepository.GetByIdWithSubscriptionsAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure(SubscriptionErrors.NotFound);
			}

			var now = _systemClock.UtcNow;
			return user.CancelSubscription(subscriptionId, now);
		}, cancellationToken);
	}
}
