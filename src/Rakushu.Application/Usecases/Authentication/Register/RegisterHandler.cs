using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.ValueObjects.Email;

namespace Rakushu.Application.Usecases.Authentication.Register;

internal sealed class RegisterHandler : IRequestHandler<RegisterCommand, Result>
{
	// DAOs
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;
	
	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly IPasswordHasher _passwordHasher;
	private readonly ISystemClock _systemClock;

	public RegisterHandler(
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

	public async Task<Result> Handle(
		RegisterCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Check unique email
			var email = request.Email.ToLower();
			var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);

			if (existingUser != null) // Expect that no user with the same email exists
			{
				return Result.Failure(UserErrors.EmailAlreadyExists);
			}

			// 3. Find role LEARNER
			var role = await _roleRepository.GetByCodeAsync(RoleCodes.Learner, cancellationToken);

			// 4. Create User
			var emailResult = Email.Create(request.Email);

					if (emailResult.IsFailure)
						return emailResult;

					var passwordHash = _passwordHasher.HashPassword(request.Password);

					var initialStatus = UserStatus.Unverified;

					var utcNow = _systemClock.UtcNow;

					var userResult = Rakushu.Domain.Entities.User.User.Create(
						request.FullName,
						emailResult.Value,
						passwordHash,
						role!.Id,
						initialStatus,
						utcNow,
						utcNow);

					if(userResult.IsFailure)
					{
						return userResult;
					}

					_userRepository.Add(userResult.Value);

					await _unitOfWork.SaveChangesAsync();
					return Result.Success();
				}, cancellationToken);
			}
}
