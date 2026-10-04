using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Series;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Domain.Entities.Video;

public sealed partial class Video : AggregateRoot<VideoId>
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
	private readonly List<MediaAsset.MediaAsset> _mediaAssets = new();
	public IReadOnlyCollection<MediaAsset.MediaAsset> MediaAssets => _mediaAssets.AsReadOnly();

	// Subtitles
	private readonly List<Subtitle.Subtitle> _subtitles = new();
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

	public static Result<Video> Create(
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
		string? sourceUrl = null)
	{
		if (string.IsNullOrWhiteSpace(title))
			return Result.Failure<Video>(VideoErrors.InvalidTitle);

		if (string.IsNullOrWhiteSpace(slug))
			return Result.Failure<Video>(VideoErrors.InvalidSlug);

		return Result.Success(new Video(
			VideoId.Create(),
			slug,
			title,
			duration,
			contentCategoryId,
			seriesId,
			sortOrder,
			sourceType,
			status,
			createdBy,
			createdAt,
			updatedAt,
			description,
			sourceUrl));
	}

	public Result Update(
		string slug,
		string title,
		TimeSpan duration,
		ContentCategoryId contentCategoryId,
		SeriesId seriesId,
		int sortOrder,
		VideoSource sourceType,
		VideoStatus status,
		DateTimeOffset updatedAt,
		string? description = null,
		string? sourceUrl = null)
	{
		if (string.IsNullOrWhiteSpace(title))
			return Result.Failure(VideoErrors.InvalidTitle);

		if (string.IsNullOrWhiteSpace(slug))
			return Result.Failure(VideoErrors.InvalidSlug);

		Slug = slug;
		Title = title;
		Duration = duration;
		ContentCategoryId = contentCategoryId;
		SeriesId = seriesId;
		SortOrder = sortOrder;
		SourceType = sourceType;
		Status = status;
		Description = description;
		SourceUrl = sourceUrl;
		UpdatedAt = updatedAt;

		return Result.Success();
	}

	public void AddMediaAsset(MediaAsset.MediaAsset asset)
	{
		_mediaAssets.Add(asset);
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void RemoveMediaAsset(MediaAsset.MediaAsset asset)
	{
		_mediaAssets.Remove(asset);
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void SetTranscript(Transcript.Transcript transcript)
	{
		Transcript = transcript;
		UpdatedAt = DateTimeOffset.UtcNow;
	}

	public void RemoveTranscript()
	{
		Transcript = null!;
		UpdatedAt = DateTimeOffset.UtcNow;
	}
}