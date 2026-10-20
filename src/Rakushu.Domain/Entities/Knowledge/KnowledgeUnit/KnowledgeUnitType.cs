using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

public enum KnowledgeUnitType
{
	Word = 1, // refers to a collection of words.
	Collocation = 2, // refers to a combination of words that are commonly used together.
	Grammar = 3, // refers to the rules and structures of a language.
	Terminology = 4, // refers to specialized terms used in a particular field or subject.
	ProperNoun = 5, // refers to a specific name of a person, place, or organization.
}