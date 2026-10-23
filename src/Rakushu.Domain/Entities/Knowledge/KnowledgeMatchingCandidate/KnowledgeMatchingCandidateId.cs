using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;

public sealed class KnowledgeMatchingCandidateId : StronglyTypedId<Guid>
{
	private KnowledgeMatchingCandidateId(Guid value) : base(value)
	{
	}

	public static KnowledgeMatchingCandidateId Create() => new(Guid.NewGuid());

	public static KnowledgeMatchingCandidateId From(Guid value) => new(value);
}