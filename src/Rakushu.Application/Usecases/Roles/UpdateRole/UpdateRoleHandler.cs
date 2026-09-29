using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Roles.UpdateRole;

internal sealed class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, Result>
{
	// DAOs
	private readonly IRoleRepository _roleRepository;
	private readonly IUserRepository _userRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public UpdateRoleHandler(
		IRoleRepository roleRepository,
		IUserRepository userRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_roleRepository = roleRepository;
		_userRepository = userRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var role = await _roleRepository.GetByIdAsync(
				RoleId.From(request.RoleId),
				cancellationToken);

			if (role is null)
			{
				return Result.Failure(RoleError.NotFound);
			}

			// Check if IsActive is being set to false
			bool isBeingDeactivated = request.IsActive.HasValue && !request.IsActive.Value && role.IsActive;

			// Determine the new IsActive value
			bool newIsActive = request.IsActive ?? role.IsActive;

			// Update the role
			role.Update(
				request.Description ?? role.Description,
				newIsActive,
				_systemClock.UtcNow);

			_roleRepository.Update(role);

			// If role is being deactivated, revoke all refresh tokens of users in this role
			if (isBeingDeactivated)
			{
				var users = await _userRepository.GetByRoleIdAsync(role.Id, cancellationToken);

				foreach (var user in users)
				{
					user.RevokeAllActiveRefreshTokens();
					_userRepository.Update(user);
				}
			}

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result.Success();
		});
	}
}
