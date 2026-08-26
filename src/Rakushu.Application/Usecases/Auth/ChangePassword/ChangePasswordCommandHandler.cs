using MediatR;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Auth.ChangePassword;

internal sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IUnitOfWork _unitOfWork;

	public ChangePasswordCommandHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		IPasswordHasher passwordHasher,
		IUnitOfWork unitOfWork)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (!userId.HasValue)
		{
			return Result.Failure(UserErrors.InvalidCredentials);
		}

		var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound);
		}

		if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
		{
			return Result.Failure(UserErrors.PasswordMismatch);
		}

		if (request.CurrentPassword == request.NewPassword)
		{
			return Result.Failure(UserErrors.SamePassword);
		}

		var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
		
		// Update password on entity (which registers UserPasswordChangedDomainEvent)
		user.UpdatePassword(newPasswordHash);

		// UnitOfWork will save changes and dispatch domain event (revoking refresh tokens) atomically
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
