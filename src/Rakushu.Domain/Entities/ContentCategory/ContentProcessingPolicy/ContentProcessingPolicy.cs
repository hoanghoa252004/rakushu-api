using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ContentCategory.ContentProcessingPolicy;

public class ContentProcessingPolicy : Entity<ContentProcessingPolicyId>
{
	public ContentCategoryId ContentCategoryId { get; private set; } = null!;
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ContentCategory
	public ContentCategory ContentCategory { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private ContentProcessingPolicy() { }

	private ContentProcessingPolicy(
		ContentProcessingPolicyId id,
		ContentCategoryId contentCategoryId,
		string code,
		string name,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null) : base(id)
	{
		ContentCategoryId = contentCategoryId;
		Code = code;
		Name = name;
		Description = description;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
