using Rakushu.Application.Usecases.Auth.ChangePassword;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.DomainEvents;
using Rakushu.Domain.Entities.User.RefreshToken;
using Rakushu.UnitTest.Fakes;

namespace Rakushu.UnitTest.Auth;

public class ChangePasswordCommandHandlerTests
{
	private readonly FakeCurrentUserContext _currentUserContext = new();
	private readonly FakeUserRepository _userRepository = new();
	private readonly FakePasswordHasher _passwordHasher = new();
	private readonly FakeUnitOfWork _unitOfWork = new();

	private readonly ChangePasswordCommandHandler _handler;

	public ChangePasswordCommandHandlerTests()
	{
		_handler = new ChangePasswordCommandHandler(
			_currentUserContext,
			_userRepository,
			_passwordHasher,
			_unitOfWork);
	}

	[Fact]
	public async Task Handle_ValidRequest_ShouldUpdatePasswordHashAndRaiseDomainEvent()
	{
		// Arrange
		var user = User.Create("charlie", "charlie@example.com", "hashed_oldpassword", RoleConstants.LearnerRoleId);
		_userRepository.Users.Add(user);
		_currentUserContext.UserId = user.Id;

		var token = RefreshToken.Create(user.Id, "active_token", DateTimeOffset.UtcNow.AddDays(7));
		_userRepository.RefreshTokens.Add(token);

		var command = new ChangePasswordCommand("oldpassword", "newpassword");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal("hashed_newpassword", user.PasswordHash);
		Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
		Assert.Contains(user.DomainEvents, e => e is UserPasswordChangedDomainEvent);

		// Test domain event handler side effect
		var eventHandler = new RevokeRefreshTokensOnPasswordChangedDomainEventHandler(_userRepository);
		await eventHandler.Handle(new UserPasswordChangedDomainEvent(user.Id), CancellationToken.None);
		Assert.True(token.IsRevoked);
	}

	[Fact]
	public async Task Handle_WrongCurrentPassword_ShouldReturnPasswordMismatchError()
	{
		// Arrange
		var user = User.Create("charlie", "charlie@example.com", "hashed_oldpassword", RoleConstants.LearnerRoleId);
		_userRepository.Users.Add(user);
		_currentUserContext.UserId = user.Id;

		var command = new ChangePasswordCommand("incorrect_password", "newpassword");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal(UserError.PasswordMismatch.Code, result.Error.Code);
	}
}
