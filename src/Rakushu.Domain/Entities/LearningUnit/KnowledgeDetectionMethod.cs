using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LearningUnit;

public enum KnowledgeDetectionMethod
{
	DictionaryMatch = 0, // 0.5 <= DictionaryMatch <= 1
	PatternMatch = 1,  // 0.0 <= PatternMatch <= 0.9
	DependencyMatch = 2, // 0.0 <= DependencyMatch <= 0.9
	Llm = 3, // always 0.0 
	Curator = 4 // always 1.0
}