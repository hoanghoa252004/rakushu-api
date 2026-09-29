using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Subtitle;

public enum SubtitleStatus
{
	Pending = 0, 
	Processing = 1,
	Ready = 2,
	Failed = 3,
	Archived = 4
}
