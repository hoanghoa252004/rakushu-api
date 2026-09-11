using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Auth.Logout;

internal sealed class LogoutHandler : IRequestHandler<LogoutCommand, Result>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public LogoutHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository, 
		IUnitOfWork unitOfWork, 
		ISystemClock systemClock
		)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Find the user by ID
			var userId = _currentUserContext.UserId;

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null) // Check whether the user exists
			{
				return Result.Failure(UserError.NotFound);
			}
			else if (user.Status != UserStatus.Active) // Check whether the user is active
			{
				return Result.Failure(UserError.UserInactiveOrBannned);
			}

			// 2. Revoke the latest refresh token
			user.RevokeAllActiveRefreshTokens();

			return Result.Success();
		}, cancellationToken);
	}
}
