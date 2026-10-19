using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Material.LearningUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;

public sealed class KnowledgeMatchingCandidate : Entity<KnowledgeMatchingCandidateId>
{
	public int StartTokenSequence { get; private set; }
	public int EndTokenSequence { get; private set; }
	public int StartIndex { get; private set; }
	public int EndIndex { get; private set; }
	public string MatchingMethod { get; private set; } = null!;
	public double? RetrievalScore { get; private set; }
	public double? MatchingConfidence { get; private set; }
	public string? MatchingExplanation { get; private set; }
	public string Status { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	private KnowledgeMatchingCandidate()
	{
	}
}