using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Admin.Users.ChangeStatus;

internal sealed class AdminChangeUserStatusCommandHandler : IRequestHandler<AdminChangeUserStatusCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AdminChangeUserStatusCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(AdminChangeUserStatusCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound);
		}

		if (Enum.TryParse<UserStatus>(request.Status, true, out var parsedStatus))
		{
			user.ChangeStatus(parsedStatus);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		return Result.Success();
	}
}
