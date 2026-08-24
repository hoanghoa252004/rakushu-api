using Rakushu.Application.Usecases.Auth.ChangePassword;
using Rakushu.Domain.Constants;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Errors;
using Rakushu.UnitTest.Fakes;

namespace Rakushu.UnitTest.Auth;

public class ChangePasswordCommandHandlerTests
{
	private readonly FakeCurrentUserContext _currentUserContext = new();
	private readonly FakeUserRepository _userRepository = new();
	private readonly FakePasswordHasher _passwordHasher = new();
	private readonly FakeRefreshTokenRepository _refreshTokenRepository = new();
	private readonly FakeUnitOfWork _unitOfWork = new();

	private readonly ChangePasswordCommandHandler _handler;

	public ChangePasswordCommandHandlerTests()
	{
		_handler = new ChangePasswordCommandHandler(
			_currentUserContext,
			_userRepository,
			_passwordHasher,
			_refreshTokenRepository,
			_unitOfWork);
	}

	[Fact]
	public async Task Handle_ValidRequest_ShouldUpdatePasswordHashAndRevokeTokens()
	{
		// Arrange
		var user = User.Create("charlie", "charlie@example.com", "hashed_oldpassword", RoleConstants.UserRoleId);
		_userRepository.Users.Add(user);
		_currentUserContext.UserId = user.Id;

		var token = RefreshToken.Create(user.Id, "active_token", DateTimeOffset.UtcNow.AddDays(7));
		_refreshTokenRepository.Tokens.Add(token);

		var command = new ChangePasswordCommand("oldpassword", "newpassword");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal("hashed_newpassword", user.PasswordHash);
		Assert.True(token.IsRevoked);
		Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
	}

	[Fact]
	public async Task Handle_WrongCurrentPassword_ShouldReturnPasswordMismatchError()
	{
		// Arrange
		var user = User.Create("charlie", "charlie@example.com", "hashed_oldpassword", RoleConstants.UserRoleId);
		_userRepository.Users.Add(user);
		_currentUserContext.UserId = user.Id;

		var command = new ChangePasswordCommand("incorrect_password", "newpassword");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal(DomainErrors.Auth.PasswordMismatch.Code, result.Error.Code);
	}
}
