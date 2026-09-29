using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ProficiencyFramework;

public sealed class ProficiencyFramework : AggregateRoot<ProficiencyFrameworkId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ProficiencyLevels:
	private readonly List<ProficiencyLevel.ProficiencyLevel> _proficiencyLevels = new();
	public IReadOnlyCollection<ProficiencyLevel.ProficiencyLevel> ProficiencyLevels => _proficiencyLevels.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private ProficiencyFramework() { }

	private ProficiencyFramework(
		ProficiencyFrameworkId id,
		string code,
		string name,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
		: base(id)
	{
		Code = code;
		Name = name;
		Description = description;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
