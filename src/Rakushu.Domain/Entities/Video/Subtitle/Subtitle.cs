using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.SupportedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Subtitle;

public sealed class Subtitle : Entity<SubtitleId>
{
	public string Code { get; private set; } = null!;
	public VideoId VideoId { get; private set; } = null!;
	public SupportedLanguageId SupportedLanguageId { get; private set; } = null!;
	public SubtitleSource SourceType { get; private set; }
	public string? StorageKey { get; private set; }
	public SubtitleStatus Status { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// Video
	public Video Video { get; private set; } = null!;

	// SubtitleItems
	private readonly List<SubtitleSegment.SubtitleSegment> _subtitleItems = new();
	public IReadOnlyCollection<SubtitleSegment.SubtitleSegment> Items => _subtitleItems.AsReadOnly();


	// SupportedLanguage
	public SupportedLanguage.SupportedLanguage SupportedLanguage { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS

	private Subtitle()
	{
	}

	private Subtitle(
		SubtitleId id,
		string code,
		VideoId videoId,
		SupportedLanguageId supportedLanguageId,
		SubtitleSource sourceType,
		SubtitleStatus status,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? storageKey = null)
		: base(id)
	{
		Code = code;
		VideoId = videoId;
		SupportedLanguageId = supportedLanguageId;
		SourceType = sourceType;
		Status = status;
		StorageKey = storageKey;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
