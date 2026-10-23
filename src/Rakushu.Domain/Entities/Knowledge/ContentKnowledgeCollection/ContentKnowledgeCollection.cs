using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;

public class ContentKnowledgeCollection : Entity<ContentKnowledgeCollectionId>
{
	public ContentCategoryId ContentCategoryId { get; private set; } = null!;
	public KnowledgeCollectionId KnowledgeCollectionId { get; private set; } = null!;
	public int Priority { get; private set; }
	public bool IsActive { get; private set; }

	// NAVIGATION PROPERTIES
	// ContentCategory
	public ContentCategory.ContentCategory ContentCategory { get; private set; } = null!;

	// KnowledgeCollection
	public KnowledgeCollection.KnowledgeCollection KnowledgeCollection { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS
	private ContentKnowledgeCollection() { }

	private ContentKnowledgeCollection(
		ContentKnowledgeCollectionId id,
		ContentCategoryId contentCategoryId,
		KnowledgeCollectionId knowledgeCollectionId,
		int priority) : base(id)
	{
		ContentCategoryId = contentCategoryId;
		KnowledgeCollectionId = knowledgeCollectionId;
		Priority = priority;
		IsActive = false;
	}

	public static ContentKnowledgeCollection Create(
		ContentCategoryId contentCategoryId,
		KnowledgeCollectionId knowledgeCollectionId,
		int priority)
	{
		return new ContentKnowledgeCollection(
			ContentKnowledgeCollectionId.Create(),
			contentCategoryId,
			knowledgeCollectionId,
			priority);
	}

	public void SetPriority(int priority)
	{
		Priority = priority;
	}

	public void SetActive(bool isActive)
	{
		IsActive = isActive;
	}
}
