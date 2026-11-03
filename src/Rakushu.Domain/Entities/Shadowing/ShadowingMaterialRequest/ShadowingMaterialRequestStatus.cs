using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

public enum ShadowingMaterialRequestStatus
{
	Pending = 1, // The request has been submitted and is awaiting review
	Processing = 2, // The request has been reviewed and approved, and the shadowing material start to create with inital status = Processing
	Rejected = 3, // The request has been reviewed and rejected, the shadowing material will not be created
	Completed = 4, // The shadowing material has been created and is now available for use, with isActive = true
}