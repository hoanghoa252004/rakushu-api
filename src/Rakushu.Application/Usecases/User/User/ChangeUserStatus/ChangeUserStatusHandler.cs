using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.User.User.ChangeUserStatus;

internal sealed class ChangeUserStatusHandler : IRequestHandler<ChangeUserStatusCommand, Result>
{
	// DAOs
	private readonly IUserRepository _userRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public ChangeUserStatusHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var userId = UserId.From(request.UserId);

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure(UserErrors.NotFound);
			}

			if (user.Role.Code == RoleCodes.SystemAdministrator)
			{
				return Result.Failure<UserDetailDto>(UserErrors.UnauthorizedResourceAccess);
			}

			if (!Enum.TryParse<UserStatus>(request.Status, true, out var status))
			{
				return Result.Failure<Guid>(UserErrors.InvalidStatus);
			}

			return user.ChangeStatus(status, _systemClock.UtcNow);
		}, cancellationToken);
	}
}
