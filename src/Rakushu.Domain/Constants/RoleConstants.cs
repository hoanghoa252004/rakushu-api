namespace Rakushu.Domain.Constants;

public static class RoleConstants
{
	public const string Admin = "Admin";
	public const string LinguisticCurator = "LinguisticCurator";
	public const string Learner = "Learner";
	public const string User = "Learner"; // Alias for default end-user

	public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
	public static readonly Guid LinguisticCuratorRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
	public static readonly Guid LearnerRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
	public static readonly Guid UserRoleId = LearnerRoleId;
}
