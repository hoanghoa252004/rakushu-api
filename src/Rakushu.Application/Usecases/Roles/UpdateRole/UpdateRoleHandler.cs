using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Application.Usecases.Roles.UpdateRole;

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
				return Result.Failure(RoleError.NotFound);
			}

			role.Update(
				request.Description ?? role.Description,
				_systemClock.UtcNow);

			_roleRepository.Update(role);

			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result.Success();
		});
	}
}
