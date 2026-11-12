using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;


public sealed class ShadowingAssessmentId : StronglyTypedId<Guid>
{
	private ShadowingAssessmentId(Guid value) : base(value)
	{
	}

	public static ShadowingAssessmentId Create() => new(Guid.NewGuid());

	public static ShadowingAssessmentId From(Guid value) => new(value);
}