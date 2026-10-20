using Rakushu.Domain.Common.Linguistic;
using Rakushu.Domain.Entities.Material.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.DependencyRelationship;

public sealed class DependencyRelationship : Linguistic<DependencyRelationshipId>
{
	// Tokens
	private readonly List<Token> _tokens = new();
	public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();

	// KnowledgeUnitElements
	private readonly List<KnowledgeUnitElement.KnowledgeUnitElement> _knowledgeUnitElements = new();
	public IReadOnlyCollection<KnowledgeUnitElement.KnowledgeUnitElement> KnowledgeUnitElements => _knowledgeUnitElements.AsReadOnly();

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
}