using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public enum MaterialType
{
	Curated = 1, // The shadowing material belongs to the system ( created by linguistic curators).
	RequestedByLearner = 2, // The shadowing material is created based on the learner's request.
}
