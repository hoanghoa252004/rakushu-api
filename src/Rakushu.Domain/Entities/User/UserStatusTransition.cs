using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User;

public static class UserStatusTransition
{
	public static readonly Dictionary<UserStatus, HashSet<UserStatus>> Transitions = new()
	{
		{
			UserStatus.Inactive, new HashSet<UserStatus>
			{
				UserStatus.Unverified
			}
		},
		{
			UserStatus.Unverified, new HashSet<UserStatus>
			{
				UserStatus.Active
			}
		},
		{
			UserStatus.Active , new HashSet<UserStatus>
			{
				UserStatus.Banned
			}
		},
		{
			UserStatus.Banned , new HashSet<UserStatus>()
		}
	};

	public static bool IsAllowed(UserStatus currentStatus, UserStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus)) // Not have status transition yet -> false
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetUserStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(UserStatus)));
	}
}
