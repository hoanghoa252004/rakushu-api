using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video;

public enum VideoSource
{
	Curated = 1,
	UploadedByLearner = 2,
	ImportedFromExternal = 3
}