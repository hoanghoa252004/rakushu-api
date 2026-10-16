using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.Speaker;

public sealed class SpeakerId : StronglyTypedId<Guid>
{
	private SpeakerId(Guid value) : base(value)
	{
	}

	public static SpeakerId Create() => new(Guid.NewGuid());

	public static SpeakerId From(Guid value) => new(value);
}