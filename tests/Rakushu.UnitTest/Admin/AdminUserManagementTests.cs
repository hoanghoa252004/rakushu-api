using Rakushu.Application.Usecases.Admin.Roles.GetRoles;
using Rakushu.Application.Usecases.Admin.Users.ChangeStatus;
using Rakushu.Application.Usecases.Admin.Users.CreateUser;
using Rakushu.Application.Usecases.Admin.Users.DeleteUser;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Application.Usecases.Admin.Users.GetUsers;
using Rakushu.Application.Usecases.Admin.Users.UpdateUser;
using Rakushu.Domain.Constants;
using Rakushu.Domain.Entities;
using Rakushu.Domain.Enums;
using Rakushu.UnitTest.Fakes;

namespace Rakushu.UnitTest.Admin;

public class AdminUserManagementTests
{
	private readonly FakeUserRepository _userRepository = new();
	private readonly FakeRoleRepository _roleRepository = new();
	private readonly FakeProfileRepository _profileRepository = new();
	private readonly FakePasswordHasher _passwordHasher = new();
	private readonly FakeUnitOfWork _unitOfWork = new();

	public AdminUserManagementTests()
	{
		_roleRepository.Roles.Add(new Role(RoleConstants.AdminRoleId, RoleConstants.Admin, "System Admin"));
		_roleRepository.Roles.Add(new Role(RoleConstants.LinguisticCuratorRoleId, RoleConstants.LinguisticCurator, "Linguistic Curator"));
		_roleRepository.Roles.Add(new Role(RoleConstants.LearnerRoleId, RoleConstants.Learner, "Learner end-user"));
	}

	[Fact]
	public async Task AdminCreateUser_ShouldAddUserAndProfile()
	{
		var handler = new AdminCreateUserCommandHandler(
			_userRepository, _roleRepository, _profileRepository, _passwordHasher, _unitOfWork);

		var command = new AdminCreateUserCommand(
			"new_student", "student@test.com", "pass123", RoleConstants.LearnerRoleId, "Student Name");

		var result = await handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Equal("new_student", result.Value.Username);
		Assert.Equal(RoleConstants.Learner, result.Value.RoleName);
		Assert.Single(_userRepository.Users);
	}

	[Fact]
	public async Task AdminChangeUserStatus_ShouldUpdateStatus()
	{
		var user = User.Create("test_user", "test@test.com", "pass", RoleConstants.LearnerRoleId);
		_userRepository.Users.Add(user);

		var handler = new AdminChangeUserStatusCommandHandler(_userRepository, _unitOfWork);
		var command = new AdminChangeUserStatusCommand(user.Id, UserStatus.Banned.ToString());

		var result = await handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Equal(UserStatus.Banned.ToString(), user.Status);
	}

	[Fact]
	public async Task AdminDeleteUser_ShouldRemoveUser()
	{
		var user = User.Create("to_delete", "delete@test.com", "pass", RoleConstants.LearnerRoleId);
		_userRepository.Users.Add(user);

		var handler = new AdminDeleteUserCommandHandler(_userRepository, _unitOfWork);
		var command = new AdminDeleteUserCommand(user.Id);

		var result = await handler.Handle(command, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Empty(_userRepository.Users);
	}

	[Fact]
	public async Task GetRoles_ShouldReturnAllRoles()
	{
		var handler = new GetRolesQueryHandler(_roleRepository);
		var result = await handler.Handle(new GetRolesQuery(), CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Equal(3, result.Value.Count);
	}
}
