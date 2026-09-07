using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Admin.Users.DeleteUser;

internal sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
{
	// DAOs
	private readonly IUserRepository _userRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public DeleteUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var userId = UserId.From(request.UserId);

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null)
			{
				return Result.Failure(UserError.NotFound);
			}

			_userRepository.Delete(user);

			return Result.Success();
		}, cancellationToken);
	}
}
