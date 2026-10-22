using Rakushu.Application.Common.Pagination;

namespace Rakushu.Application.Usecases.Material.Transcript;

//public sealed record TranscriptBasicDto(
//	Guid Id,
//	DateTimeOffset UpdatedAt,
//	PaginatedList<TranscriptSegmentBasicDto> Segments);

public sealed record TranscriptSegmentBasicDto(
	Guid Id,
	int Sequence,
	TimeSpan StartTime,
	TimeSpan EndTime,
	SpeakerBasicDto Speaker,
	string Text,
	SubtitleBasicDto Subtitle,
	IReadOnlyCollection<LearningUnitBasicDto> LearningUnits);

public sealed record SpeakerBasicDto(
	string SpeakerLabel,
	Guid? Id,
	string? Name,
	string? JapaneseName,
	string? ThemeColor,
	string? AvatarKey,
	string? Description
	);

public sealed record SubtitleBasicDto(
	Guid Id,
	int Sequence,
	string LanguageCode,
	string Text
	);

public sealed record LearningUnitBasicDto(
	Guid Id,
	int StartIndex,
	int EndIndex,
	string Collection, // IT
	string Type, // Terminology
	string Expression, // システム開発
	string Meaning // Phát triển hệ
	);