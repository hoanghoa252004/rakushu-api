using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public static class ShadowingMaterialStatusTransition
{
	public static readonly Dictionary<ShadowingMaterialStatus, HashSet<ShadowingMaterialStatus>> Transitions = new()
	{
		{
			ShadowingMaterialStatus.Processing, new HashSet<ShadowingMaterialStatus>
			{

				ShadowingMaterialStatus.Reviewing
			}
		},
		{
			ShadowingMaterialStatus.Reviewing, new HashSet<ShadowingMaterialStatus>
			{
				ShadowingMaterialStatus.Ready
			}
		},
		{
			ShadowingMaterialStatus.Ready , new HashSet<ShadowingMaterialStatus>
			{
				ShadowingMaterialStatus.Archived
			}
		},
		{
			ShadowingMaterialStatus.Archived , new HashSet<ShadowingMaterialStatus>()
		}
	};

	public static bool IsAllowed(ShadowingMaterialStatus currentStatus, ShadowingMaterialStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus)) // Not have status transition yet -> false
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetMaterialStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(ShadowingMaterialStatus)));
	}
}
