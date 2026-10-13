using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

public enum KnowledgeCollectionType
{
	Vocabulary = 1, // refers to a collection of words.
	Grammar = 2, // refers to the rules and structures of a language.
	Terminology = 3, // refers to specialized terms used in a particular field or subject.
	ProperNoun = 4, // refers to a specific name of a person, place, or organization.
	Mixed = 5 // refers to a collection that contains a combination of different types of knowledge, such as vocabulary, grammar, and terminology.
}