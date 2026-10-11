using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video;

public enum VideoStatus
{
	Draft = 1, // Video upload by Curator, is in draft mode and not available for using in the system (e.g., for learners to watch)
	Pending = 2, // Video upload by Learner with VideoSource UploadedByLearner || ImportedFromExternal, is pending approval by Linguistic Curator. If VideoSource is Curated, then it is automatically approved and set to Processing.
	Rejected = 3, // Video is rejected by Linguistic Curator and not available for using in the system (e.g., for learners to watch)
	Processing = 4, // Video with VideoSource Curated, is being processed by system (e.g., transcoding, thumbnail generation, etc.)
	Reviewing = 5, // Video is being reviewed by Linguistic Curator
	Ready = 6, // Video is ready and available for using in the system (e.g., for learners to watch)
	Archived = 7, // Video is archived and not available for using in the system (e.g., for learners to watch)
	
}
