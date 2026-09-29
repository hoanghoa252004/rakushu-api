using Rakushu.Domain.Common;
using Rakushu.Domain.SupportedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning.KnowledgeMeaningDetail;

public sealed class KnowledgeMeaningDetail : Entity<KnowledgeMeaningDetailId>
{
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;
	public SupportedLanguageId SupportedLanguageId { get; private set; } = null!;
	public string TranslatedMeaning { get; private set; } = null!;
	public string? NativeNote { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// KnowledgeMeaning
	public KnowledgeMeaning KnowledgeMeaning { get; private set; } = null!;

	// SupportedLanguage
	public SupportedLanguage.SupportedLanguage SupportedLanguage { get; private set; } = null!;


	private KnowledgeMeaningDetail()
	{
	}


	private KnowledgeMeaningDetail(
		KnowledgeMeaningDetailId id,
		KnowledgeMeaningId knowledgeMeaningId,
		SupportedLanguageId supportedLanguageId,
		string translatedMeaning,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? nativeNote = null)
		: base(id)
	{
		KnowledgeMeaningId = knowledgeMeaningId;
		SupportedLanguageId = supportedLanguageId;
		TranslatedMeaning = translatedMeaning;
		NativeNote = nativeNote;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}