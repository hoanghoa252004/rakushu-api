using Rakushu.Application.Usecases.Auth.Register;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.UnitTest.Fakes;

namespace Rakushu.UnitTest.Auth;

public class RegisterCommandHandlerTests
{
	private readonly FakeUserRepository _userRepository = new();
	private readonly FakeRoleRepository _roleRepository = new();
	private readonly FakePasswordHasher _passwordHasher = new();
	private readonly FakeJwtTokenGenerator _jwtTokenGenerator = new();
	private readonly FakeUnitOfWork _unitOfWork = new();

	private readonly RegisterCommandHandler _handler;

	public RegisterCommandHandlerTests()
	{
		_roleRepository.Roles.Add(new Role(RoleConstants.LearnerRoleId, RoleConstants.Learner, "Learner user"));

		_handler = new RegisterCommandHandler(
			_userRepository,
			_roleRepository,
			_passwordHasher,
			_jwtTokenGenerator,
			_unitOfWork);
	}

	[Fact]
	public async Task Handle_ValidRequest_ShouldCreateUserAndProfileAndReturnTokens()
	{
		// Arrange
		var command = new RegisterCommand("john_doe", "john@example.com", "secret123", "John Doe");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.NotNull(result.Value);
		Assert.Equal("john_doe", result.Value.Username);
		Assert.Equal("john@example.com", result.Value.Email);
		Assert.Equal(RoleConstants.Learner, result.Value.Role);
		Assert.Equal("John Doe", result.Value.DisplayName);
		Assert.Equal("fake_access_token", result.Value.AccessToken);
		Assert.Single(_userRepository.Users);
		Assert.NotNull(_userRepository.Users.First().Profile);
		Assert.Single(_userRepository.RefreshTokens);
		Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
	}

	[Fact]
	public async Task Handle_DuplicateEmail_ShouldReturnEmailAlreadyExistsError()
	{
		// Arrange
		_userRepository.Users.Add(User.Create("existing_user", "john@example.com", "hash", RoleConstants.LearnerRoleId));
		var command = new RegisterCommand("john_doe", "john@example.com", "secret123");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal(UserError.EmailAlreadyExists.Code, result.Error.Code);
	}

	[Fact]
	public async Task Handle_DuplicateUsername_ShouldReturnUsernameAlreadyExistsError()
	{
		// Arrange
		_userRepository.Users.Add(User.Create("john_doe", "existing@example.com", "hash", RoleConstants.LearnerRoleId));
		var command = new RegisterCommand("john_doe", "new@example.com", "secret123");

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal(UserError.UsernameAlreadyExists.Code, result.Error.Code);
	}
}
