using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Video;

public enum VideoSource
{
	Curated = 1, // Video upload by Linguistic Curator
	UploadedByLearner = 2, // Video upload by Learner
	ImportedFromExternal = 3 // Video imported by Learner from external source (e.g., YouTube, Vimeo, etc.)
}