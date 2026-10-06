using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.LinguisticKnowledge.LinguisticKnowledgePattern.LinguisticKnowledgePatternRelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

public sealed class DependencyRelationship : Linguistic<DependencyRelationshipId>
{
	// NAVIGATION PROPERTIES
	// BunsetsuDependencyRelationships
	private readonly List<BunsetsuDependencyRelationship> _bunsetsuDependencyRelationships = new List<BunsetsuDependencyRelationship>();
	public IReadOnlyCollection<BunsetsuDependencyRelationship> BunsetsuDependencyRelationships => _bunsetsuDependencyRelationships.AsReadOnly();

	// Tokens
	private readonly List<Token> _tokens = new();
	public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();

	// KnowledgePatternRelations
	private readonly List<LinguisticKnowledgePatternRelation> _knowledgePatternRelations = new();
	public IReadOnlyCollection<LinguisticKnowledgePatternRelation> KnowledgePatternRelations => _knowledgePatternRelations.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private DependencyRelationship()
	{
	}

	private DependencyRelationship(
		DependencyRelationshipId id,
		string code,
		string name,
		string japaneseName,
		string? description)
		: base(
			id,
			code,
			name,
			japaneseName,
			description)
	{
	}

	public static DependencyRelationship Create(
		DependencyRelationshipId id,
		string code,
		string name,
		string japaneseName,
		string? description = null)
	{
		return new DependencyRelationship(
			id,
			code,
			name,
			japaneseName,
			description);
	}

	public void Update(
		string name,
		string japaneseName,
		string? description)
	{
		base.Update(name, japaneseName, description);
	}
}