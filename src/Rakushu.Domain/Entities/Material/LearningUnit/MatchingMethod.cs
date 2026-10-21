using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.LearningUnit;

public enum MatchingMethod
{
	KnowledgeMatching = 0, // 0.5 <= KnowledgeMatching <= 1
	CuratorDefine = 4 // always 1.0
}