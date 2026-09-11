using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Auth.Login;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Auth.ChangePassword;

internal sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result>
{
	// USER CONTEXT
	private readonly ICurrentUserContext _currentUserContext;

	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IUnitOfWork _unitOfWork;

	//SERVICES
	private readonly IPasswordHasher _passwordHasher;

	public ChangePasswordHandler(
		ICurrentUserContext currentUserContext,
		IUserRepository userRepository,
		IUnitOfWork unitOfWork,
		IPasswordHasher passwordHasher
		)
	{
		_currentUserContext = currentUserContext;
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
		_passwordHasher = passwordHasher;
	}

	public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync( async () =>
		{
			// 1. Find the user by ID
			var userId = _currentUserContext.UserId;

			var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

			if (user == null) // Check whether the user exists
			{
				return Result.Failure<CredentialResponseDto>(UserError.NotFound);
			}

			// 2. Verify the current password
			if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
			{
				return Result.Failure(UserError.PasswordMismatch);
			}

			// 3. Check if the new password is the same as the current password
			if (request.CurrentPassword == request.NewPassword)
			{
				return Result.Failure(UserError.SamePassword);
			}

			// 4. Update new password
			var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);

			user.ChangePassword(newPasswordHash);

			return Result.Success();
		}, cancellationToken);
	}
}
