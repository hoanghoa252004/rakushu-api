using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSession;

public enum ShadowingSessionStatus
{
	Scheduled = 1,
	InProgress = 2,
	Completed = 3,
	Cancelled = 4
}