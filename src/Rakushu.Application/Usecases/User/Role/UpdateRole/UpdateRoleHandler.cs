using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.User.Role.UpdateRole;

internal sealed class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, Result>
{
	// DAOs
	private readonly IRoleRepository _roleRepository;
	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public UpdateRoleHandler(
		IRoleRepository roleRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_roleRepository = roleRepository;
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
				return Result.Failure(RoleErrors.NotFound);
			}

			if(role.Code == RoleCodes.SystemAdministrator)
			{
				return Result.Failure(RoleErrors.CannotUpdateSystemAdministrator);
			}

			role.Update(
				request.Name,
				request.IsActive,
				_systemClock.UtcNow,
				request.Description);

			// Inactive all user in this role if the role is inactive
			if (role.IsActive == false)
			{
				var items = role.Users.Where(u => u.Status == UserStatus.Active).ToList();
				foreach (var user in items)
				{
					user.Deactivate(_systemClock.UtcNow);
				}
			}

			return Result.Success();
		}, cancellationToken);
	}
}
