using Rakushu.Domain.Common;
using System;

namespace Rakushu.Domain.Entities.OovCandidate;

public sealed class OovCandidateId : StronglyTypedId<Guid>
{
	private OovCandidateId(Guid value) : base(value)
	{
	}

	public static OovCandidateId Create() => new(Guid.NewGuid());

	public static OovCandidateId From(Guid value) => new(value);
}
