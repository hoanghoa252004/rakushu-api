using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video;
public static class VideoStatusTransition
{
	public static readonly Dictionary<VideoStatus, HashSet<VideoStatus>> Transitions = new()
	{
		{
			VideoStatus.Draft, new HashSet<VideoStatus>
			{
				VideoStatus.Processing
			}
		},
		{
			VideoStatus.Pending, new HashSet<VideoStatus>
			{
				VideoStatus.Processing,
				VideoStatus.Rejected
			}
		},
		{
			VideoStatus.Processing , new HashSet<VideoStatus>
			{
				VideoStatus.Reviewing
			}
		},
		{
			VideoStatus.Reviewing , new HashSet<VideoStatus>
			{
				VideoStatus.Ready
			}
		},
		{
			VideoStatus.Rejected , new HashSet<VideoStatus>()
		},
		{
			VideoStatus.Archived , new HashSet<VideoStatus>()
		}
	};

	public static bool IsAllowed(VideoStatus currentStatus, VideoStatus newStatus)
	{
		if (!Transitions.ContainsKey(currentStatus)) // Not have status transition yet -> false
		{
			return false;
		}

		return Transitions[currentStatus].Contains(newStatus);
	}

	public static string GetVideoStatuses()
	{
		return string.Join(", ", Enum.GetNames(typeof(VideoStatus)));
	}
}
