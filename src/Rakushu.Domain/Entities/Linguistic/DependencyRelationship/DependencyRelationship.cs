using Rakushu.Domain.Entities.Knowledge.KnowledgePattern.KnowledgePatternRelation;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.BunsetsuDependencyRelationship;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.Linguistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Linguistic.DependencyRelationship;

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
	private readonly List<KnowledgePatternRelation> _knowledgePatternRelations = new();
	public IReadOnlyCollection<KnowledgePatternRelation> KnowledgePatternRelations => _knowledgePatternRelations.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private DependencyRelationship()
	{
	}

	private DependencyRelationship(
		DependencyRelationshipId id,
		string code,
		string name,
		string vietnameseName,
		string? description)
		: base(
			id,
			code,
			name,
			vietnameseName,
			description)
	{
	}

	public static DependencyRelationship Create(
		DependencyRelationshipId id,
		string code,
		string name,
		string vietnameseName,
		string? description = null)
	{
		return new DependencyRelationship(
			id,
			code,
			name,
			vietnameseName,
			description);
	}
}