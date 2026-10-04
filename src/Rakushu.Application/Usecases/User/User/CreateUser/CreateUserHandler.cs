using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.ValueObjects.Email;

namespace Rakushu.Application.Usecases.User.User.CreateUser;

internal sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<UserId>>
{// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IPasswordHasher _passwordHasher;
	private readonly ISystemClock _systemClock;

	public CreateUserHandler(
		IUserRepository userRepository,
		IRoleRepository roleRepository,
		IUnitOfWork unitOfWork,
		IPasswordHasher passwordHasher,
		ISystemClock systemClock
		)
	{
		_userRepository = userRepository;
		_roleRepository = roleRepository;
		_unitOfWork = unitOfWork;
		_passwordHasher = passwordHasher;
		_systemClock = systemClock;
	}

	public async Task<Result<UserId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Check unique email
			var email = request.Email.ToLower();
			var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);

			if (existingUser != null) // Expect that no user with the same email exists
			{
				return Result.Failure<UserId>(UserErrors.EmailAlreadyExists);
			}

			// 3. Find role
			var roleId = RoleId.From(request.RoleId);

			var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

			if (role == null) 
			{
				return Result.Failure<UserId>(RoleErrors.NotFound);
			}

			// 4. Create User
			var emailResult = Email.Create(request.Email);

			if (emailResult.IsFailure)
			{
				//return emailResult;
				return Result.Failure<UserId>(emailResult.Error);
			}	

			var passwordHash = _passwordHasher.HashPassword(request.Password);

			var initialStatus = UserStatus.Unverified;

			var utcNow = _systemClock.UtcNow;

			var userResult = Rakushu.Domain.Entities.User.User.Create(
				emailResult.Value,
				passwordHash,
				role!.Id,
				initialStatus,
				utcNow,
				utcNow);

			if (userResult.IsFailure)
			{
				//return emailResult;
				return Result.Failure<UserId>(userResult.Error);
			}

			_userRepository.Add(userResult.Value);

			return Result.Success(userResult.Value.Id);
		}, cancellationToken);
	}
}
