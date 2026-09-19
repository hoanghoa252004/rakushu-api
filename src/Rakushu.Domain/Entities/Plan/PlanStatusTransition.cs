using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Plan;

public static class PlanStatusTransition
{
	public static readonly Dictionary<PlanStatus, HashSet<PlanStatus>> Transitions = new()
	{
		{
			PlanStatus.Draft, new HashSet<PlanStatus>
			{
				PlanStatus.Active,
				PlanStatus.Inactive
			}
		},
		{
			PlanStatus.Active, new HashSet<PlanStatus>
			{
				PlanStatus.Inactive
			}
		},
		{
			PlanStatus.Inactive, new HashSet<PlanStatus>
			{
				PlanStatus.Active,
				PlanStatus.Archived
			}
		},
		{
			PlanStatus.Archived , new HashSet<PlanStatus>()
		}
	};

	public static bool IsAllowed(PlanStatus currentStatus, PlanStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus)) // Not have status transition yet -> false
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetPlanStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(PlanStatus)));
	}
}