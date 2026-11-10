using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

public enum ShadowingMaterialStatus
{
	Processing = 1, // First created
	Ready = 3, // After processing, ready for shadowing
	Archived = 4 // Soft delete
}