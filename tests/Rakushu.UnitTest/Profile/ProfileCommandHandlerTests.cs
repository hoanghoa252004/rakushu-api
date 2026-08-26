using Rakushu.Application.Usecases.Profile.GetMyProfile;
using Rakushu.Application.Usecases.Profile.UpdateMyProfile;
using Rakushu.Domain.Entities.Role;
using Rakushu.Domain.Entities.User;
using Rakushu.UnitTest.Fakes;

namespace Rakushu.UnitTest.ProfileTests;

public class ProfileCommandHandlerTests
{
	private readonly FakeCurrentUserContext _currentUserContext = new();
	private readonly FakeUserRepository _userRepository = new();
	private readonly FakeUnitOfWork _unitOfWork = new();

	private readonly GetMyProfileQueryHandler _getQueryHandler;
	private readonly UpdateMyProfileCommandHandler _updateCommandHandler;

	public ProfileCommandHandlerTests()
	{
		_getQueryHandler = new GetMyProfileQueryHandler(_currentUserContext, _userRepository);
		_updateCommandHandler = new UpdateMyProfileCommandHandler(_currentUserContext, _userRepository, _unitOfWork);
	}

	[Fact]
	public async Task GetMyProfile_ShouldReturnProfileDetails()
	{
		// Arrange
		var user = User.Create("eva", "eva@example.com", "hash", RoleConstants.LearnerRoleId);
		user.SetProfile(Profile.Create(user.Id, "Eva Green", null, "Hello world", "English", "Japanese"));
		_userRepository.Users.Add(user);
		_currentUserContext.UserId = user.Id;

		// Act
		var result = await _getQueryHandler.Handle(new GetMyProfileQuery(), CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal("Eva Green", result.Value.DisplayName);
		Assert.Equal("Japanese", result.Value.LearningLanguage);
	}

	[Fact]
	public async Task UpdateMyProfile_ShouldUpdateProfileFields()
	{
		// Arrange
		var user = User.Create("eva", "eva@example.com", "hash", RoleConstants.LearnerRoleId);
		var profile = Profile.Create(user.Id, "Old Name");
		user.SetProfile(profile);
		_userRepository.Users.Add(user);
		_currentUserContext.UserId = user.Id;

		var command = new UpdateMyProfileCommand("New Name", "https://img.com/avatar.png", "New Bio", "Vietnamese", "Japanese");

		// Act
		var result = await _updateCommandHandler.Handle(command, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal("New Name", result.Value.DisplayName);
		Assert.Equal("https://img.com/avatar.png", result.Value.AvatarUrl);
		Assert.Equal("New Bio", result.Value.Bio);
		Assert.Equal("Vietnamese", result.Value.NativeLanguage);
		Assert.Equal("Japanese", result.Value.LearningLanguage);
		Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
	}
}
