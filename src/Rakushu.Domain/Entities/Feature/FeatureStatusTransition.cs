using Rakushu.Domain.Entities.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Feature;

public static class FeatureStatusTransition
{
	public static readonly Dictionary<FeatureStatus, HashSet<FeatureStatus>> Transitions = new()
	{
		{
			FeatureStatus.Draft, new HashSet<FeatureStatus>
			{
				FeatureStatus.Active,
				FeatureStatus.Inactive
			}
		},
		{
			FeatureStatus.Active, new HashSet<FeatureStatus>
			{
				FeatureStatus.Inactive
			}
		},
		{
			FeatureStatus.Inactive, new HashSet<FeatureStatus>
			{
				FeatureStatus.Active,
				FeatureStatus.Archived
			}
		},
		{
			FeatureStatus.Archived , new HashSet<FeatureStatus>()
		}
	};

	public static bool IsAllowed(FeatureStatus currentStatus, FeatureStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus)) // Not have status transition yet -> false
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetFeatureStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(FeatureStatus)));
	}
}