using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Series;

public static class SeriesErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SERIES.NOT_FOUND", "The series was not found.");

	public static readonly Error HasVideos = Error.Conflict(
		"SERIES.HAS_VIDEOS",
		"Cannot delete series that has videos. Please delete or move all videos first.");
}
