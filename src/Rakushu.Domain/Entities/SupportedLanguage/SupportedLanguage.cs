using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning.KnowledgeMeaningDetail;
using Rakushu.Domain.Entities.User.Profile;
using Rakushu.Domain.Entities.Video.Subtitle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.SupportedLanguage;

public sealed class SupportedLanguage : AggregateRoot<SupportedLanguageId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string NativeName { get; private set; } = null!;
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// KnowledgeMeanings
	private readonly List<KnowledgeMeaningDetail> _knowledgeMeaningDetails = new();
	public IReadOnlyCollection<KnowledgeMeaningDetail> KnowledgeMeaningDetails => _knowledgeMeaningDetails.AsReadOnly();


	// Profiles
	private readonly List<Profile> _profiles = new();
	public IReadOnlyCollection<Profile> Profiles => _profiles.AsReadOnly();

	// Subtitles
	private readonly List<Subtitle> _subtitles = new();
	public IReadOnlyCollection<Subtitle> Subtitles => _subtitles.AsReadOnly();

	private SupportedLanguage() { }

	private SupportedLanguage(
		SupportedLanguageId id,
		string code,
		string name,
		string nativeName,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		Code = code;
		Name = name;
		NativeName = nativeName;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
