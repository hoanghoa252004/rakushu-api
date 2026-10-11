using Rakushu.Domain.Common;
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
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }


	// NAVIGATION PROPERTIES
	// Video
	public Video Video { get; private set; } = null!;

	// SubtitleItems
	private readonly List<SubtitleSegment.SubtitleSegment> _subtitleItems = new();
	public IReadOnlyCollection<SubtitleSegment.SubtitleSegment> Items => _subtitleItems.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS

	private Subtitle()
	{
	}

	private Subtitle(
		SubtitleId id,
		string code,
		VideoId videoId,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? storageKey = null)
		: base(id)
	{
		Code = code;
		VideoId = videoId;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
