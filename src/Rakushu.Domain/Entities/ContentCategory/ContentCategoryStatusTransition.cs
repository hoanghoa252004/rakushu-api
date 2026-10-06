namespace Rakushu.Domain.Entities.ContentCategory;

public static class ContentCategoryStatusTransition
{
	public static readonly Dictionary<ContentCategoryStatus, HashSet<ContentCategoryStatus>> Transitions = new()
	{
		{
			ContentCategoryStatus.Draft, new HashSet<ContentCategoryStatus>
			{
				ContentCategoryStatus.Active,
				ContentCategoryStatus.Inactive
			}
		},
		{
			ContentCategoryStatus.Active, new HashSet<ContentCategoryStatus>
			{
				ContentCategoryStatus.Inactive
			}
		},
		{
			ContentCategoryStatus.Inactive, new HashSet<ContentCategoryStatus>
			{
				ContentCategoryStatus.Active,
				ContentCategoryStatus.Archived
			}
		},
		{
			ContentCategoryStatus.Archived, new HashSet<ContentCategoryStatus>()
		}
	};

	public static bool IsAllowed(ContentCategoryStatus currentStatus, ContentCategoryStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus))
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetAllowedStatuses(ContentCategoryStatus currentStatus)
	{
		if (!Transitions.ContainsKey(currentStatus))
		{
			return "No transitions available";
		}

		return string.Join(", ", Transitions[currentStatus]);
	}
}
