using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Admin.Users.ChangeUserStatus;

internal sealed class ChangeUserStatusHandler : IRequestHandler<ChangeUserStatusCommand, Result>
{
	// DAOs
	private readonly IUserRepository _userRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public ChangeUserStatusHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var userId = UserId.From(request.UserId);

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure(UserError.NotFound);
			}

			if (!Enum.TryParse<UserStatus>(request.Status, true, out var status))
			{
				return Result.Failure<Guid>(UserError.InvalidStatus);
			}

			return user.ChangeStatus(status);
		}, cancellationToken);
	}
}
