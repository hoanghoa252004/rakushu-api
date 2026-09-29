using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video;

public sealed class Video : AggregateRoot<VideoId>
{
	public string Slug { get; private set; } = null!;
	public string Title { get; private set; } = null!;
	public string? Description { get; private set; }
	public TimeSpan Duration { get; private set; }
	public ContentCategoryId ContentCategoryId { get; private set; } = null!;
	public SeriesId SeriesId { get; private set; } = null!;
	public int SortOrder { get; private set; }
	public VideoSource SourceType { get; private set; }
	public string? SourceUrl { get; private set; }
	public VideoStatus Status { get; private set; }
	public UserId CreatedBy { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ContentCategory
	public ContentCategory.ContentCategory ContentCategory { get; private set; } = null!;

	// User (CreatedBy)
	public User.User CreatedByUser { get; private set; } = null!;

	// Transcript
	public Transcript.Transcript Transcript { get; private set; } = null!;

	// Serie
	public Series.Series Series { get; private set; } = null!;

	// MediaAssets
	private readonly List<MediaAsset.MediaAsset> _mediaAssets = new List<MediaAsset.MediaAsset>();
	public IReadOnlyCollection<MediaAsset.MediaAsset> MediaAssets => _mediaAssets.AsReadOnly();

	// Subtitles
	private readonly List<Subtitle.Subtitle> _subtitles = new List<Subtitle.Subtitle>();
	public IReadOnlyCollection<Subtitle.Subtitle> Subtitles => _subtitles.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private Video() { }

	private Video(
		VideoId id,
		string slug,
		string title,
		TimeSpan duration,
		ContentCategoryId contentCategoryId,
		SeriesId seriesId,
		int sortOrder,
		VideoSource sourceType,
		VideoStatus status,
		UserId createdBy,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null,
		string? sourceUrl = null) : base(id)
	{
		Slug = slug;
		Title = title;
		Description = description;
		Duration = duration;
		ContentCategoryId = contentCategoryId;
		SeriesId = seriesId;
		SortOrder = sortOrder;
		SourceType = sourceType;
		SourceUrl = sourceUrl;
		Status = status;
		CreatedBy = createdBy;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}