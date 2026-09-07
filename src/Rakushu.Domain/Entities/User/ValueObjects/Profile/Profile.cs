using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.ValueObjects.Profile;

public sealed class Profile : ValueObject
{
	// MAIN PROPERTIES
	public string FullName { get; private set; } = null!;
	public string? AvatarKey { get; private set; }
	public string? NativeLanguage { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS----------
	private Profile(
		string fullName,
		string? avatarKey = null,
		string? nativeLanguage = null)
	{
		FullName = fullName;
		AvatarKey = avatarKey;
		NativeLanguage = nativeLanguage;
	}

	public static Result<Profile> Create(
		string fullName,
		string? nativeLanguage = null,
		string? avatarKey = null)
	{
		if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 50)
			return Result.Failure<Profile>(ProfileErrors.InvalidFullName);

		Profile profile = new Profile(
			fullName,
			avatarKey,
			nativeLanguage);

		return Result.Success(profile);
	}

	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return FullName;
		yield return AvatarKey;
		yield return NativeLanguage;
	}
}
