using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAssessment.ShadowingAssessmentDetail;

public sealed class ShadowingAssessmentDetailId : StronglyTypedId<Guid>
{
	private ShadowingAssessmentDetailId(Guid value) : base(value)
	{
	}

	public static ShadowingAssessmentDetailId Create() => new(Guid.NewGuid());

	public static ShadowingAssessmentDetailId From(Guid value) => new(value);
}