using Rakushu.Domain.Entities.Identity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

public static class ShadowingMaterialRequestStatusTransition
{
	public static readonly Dictionary<ShadowingMaterialRequestStatus, HashSet<ShadowingMaterialRequestStatus>> Transitions = new()
	{
		{
			ShadowingMaterialRequestStatus.Pending, new HashSet<ShadowingMaterialRequestStatus>
			{

				ShadowingMaterialRequestStatus.Processing,
				ShadowingMaterialRequestStatus.Rejected
			}
		},
		{
			ShadowingMaterialRequestStatus.Processing, new HashSet<ShadowingMaterialRequestStatus>
			{
				ShadowingMaterialRequestStatus.Completed
			}
		},
		{
			ShadowingMaterialRequestStatus.Completed , new HashSet<ShadowingMaterialRequestStatus>()
		},
		{
			ShadowingMaterialRequestStatus.Rejected , new HashSet<ShadowingMaterialRequestStatus>()
		}
	};

	public static bool IsAllowed(ShadowingMaterialRequestStatus currentStatus, ShadowingMaterialRequestStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus)) // Not have status transition yet -> false
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetMaterialRequestStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(ShadowingMaterialRequestStatus)));
	}
}
