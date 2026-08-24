using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Errors;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Admin.Users.DeleteUser;

internal sealed class AdminDeleteUserCommandHandler : IRequestHandler<AdminDeleteUserCommand, Result>
{
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AdminDeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(AdminDeleteUserCommand request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
		if (user is null)
		{
			return Result.Failure(DomainErrors.User.NotFound);
		}

		_userRepository.Delete(user);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
