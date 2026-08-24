using Rakushu.Application.Usecases.Auth.Login;
using Rakushu.Domain.Constants;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Enums;
using Rakushu.Domain.Errors;
using Rakushu.UnitTest.Fakes;

namespace Rakushu.UnitTest.Auth;

public class LoginCommandHandlerTests
{
	private readonly FakeUserRepository _userRepository = new();
	private readonly FakeRefreshTokenRepository _refreshTokenRepository = new();
	private readonly FakePasswordHasher _passwordHasher = new();
	private readonly FakeJwtTokenGenerator _jwtTokenGenerator = new();
	private readonly FakeUnitOfWork _unitOfWork = new();

	private readonly LoginCommandHandler _handler;

	public LoginCommandHandlerTests()
	{
		_handler = new LoginCommandHandler(
			_userRepository,
			_refreshTokenRepository,
			_passwordHasher,
			_jwtTokenGenerator,
			_unitOfWork);
	}

	[Fact]
	public async Task Handle_ValidCredentials_ShouldReturnLoginResponseWithTokens()
	{
		// Arrange
		var user = User.Create("alice", "alice@example.com", "hashed_password123", RoleConstants.UserRoleId);
		user.SetProfile(Profile.Create(user.Id, "Alice Wonderland"));
		_userRepository.Users.Add(user);

		var command = new LoginCommand("alice@example.com", "password123");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("alice", result.Value.Username);
		Assert.Equal("alice@example.com", result.Value.Email);
		Assert.Equal("fake_access_token", result.Value.AccessToken);
		Assert.Single(_refreshTokenRepository.Tokens);
	}

	[Fact]
	public async Task Handle_InvalidPassword_ShouldReturnInvalidCredentialsError()
	{
		// Arrange
		var user = User.Create("alice", "alice@example.com", "hashed_correct_pass", RoleConstants.UserRoleId);
		_userRepository.Users.Add(user);

		var command = new LoginCommand("alice@example.com", "wrong_pass");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal(DomainErrors.Auth.InvalidCredentials.Code, result.Error.Code);
	}

	[Fact]
	public async Task Handle_BannedUser_ShouldReturnUserInactiveError()
	{
		// Arrange
		var user = User.Create("banned_user", "banned@example.com", "hashed_pass", RoleConstants.UserRoleId, status: UserStatus.Banned.ToString());
		_userRepository.Users.Add(user);

		var command = new LoginCommand("banned@example.com", "pass");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal(DomainErrors.Auth.UserInactive.Code, result.Error.Code);
	}
}
