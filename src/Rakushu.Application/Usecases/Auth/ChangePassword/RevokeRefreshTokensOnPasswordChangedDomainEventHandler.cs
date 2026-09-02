using MediatR;
using Rakushu.Domain.Entities.User.DomainEvents;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Auth.ChangePassword;

internal sealed class RevokeRefreshTokensOnPasswordChangedDomainEventHandler
	: INotificationHandler<UserPasswordChangedDomainEvent>
{
	private readonly IUserRepository _userRepository;

	public RevokeRefreshTokensOnPasswordChangedDomainEventHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task Handle(UserPasswordChangedDomainEvent notification, CancellationToken cancellationToken)
	{
		await _userRepository.RevokeUserRefreshTokensAsync(notification.UserId, cancellationToken);
	}
}
