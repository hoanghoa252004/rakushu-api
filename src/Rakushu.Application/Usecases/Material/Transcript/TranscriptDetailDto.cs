using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.DependencyRelationship.Common;
using Rakushu.Application.Usecases.Knowledge.JapanesePartOfSpeech.Common;

namespace Rakushu.Application.Usecases.Material.Transcript;

//public sealed record TranscriptDetailDto(
//	Guid Id,
//	Guid VideoId,
//	string FullText,
//	string SourceType,
//	string? StorageKey,
//	DateTimeOffset CreatedAt,
//	DateTimeOffset UpdatedAt,
//	PaginatedList<TranscriptSegmentDetailDto> Segments
//	);

public sealed record TranscriptSegmentDetailDto(
	Guid Id,
	int Sequence,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string Text,
	TranscriptSpeakerDetailDto TranscriptSpeaker,
	SubtitleDetailDto Subtitle,
	IReadOnlyCollection<LearningUnitDetailDto> LearningUnits,
	IReadOnlyCollection<TranscriptItemDto> TranscriptItems,
	IReadOnlyCollection<BunsetsuDetailDto> Bunsetsus
	);

public sealed record TranscriptItemDto(
	Guid Id,
	int Sequence,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string Type,
	string Text,
	decimal Confidence
	);

public sealed record BunsetsuDetailDto(
	Guid Id,
	int Sequence,
	string Text,
	int StartIndex,
	int EndIndex,
	IReadOnlyCollection<TokenDetailDto> Tokens
	);

public sealed record TokenDetailDto(
	Guid Id,
	int Sequence,
	int StartIndex,
	int EndIndex,
	string Surface,
	string Lemma,
	string Reading,
	JapanesePartOfSpeechDto JapanesePartOfSpeech,
	DependencyRelationshipDto DependencyRelationship
	);

public sealed record TranscriptSpeakerDetailDto(
	string SpeakerLabel,
	Guid? Id,
	string? Name,
	string? JapaneseName,
	string? ThemeColor,
	string? AvatarKey,
	string? Description
	);

public sealed record SubtitleDetailDto(
	Guid Id,
	int Sequence,
	string LanguageCode,
	string Text
	);

public sealed record LearningUnitDetailDto(
	Guid Id,
	KnowledgeUnitDto KnowledgeUnit,
	KnowledgeMeaningDto KnowledgeMeaning,
	int StartIndex,
	int EndIndex,
	string DetectionMethod,
	double Confidence,
	bool IsReviewed
	);

public sealed record KnowledgeUnitDto(
	Guid Id,
	string KnowledgeType, // Terminology
	string SourceType, // Defined
	string ProficiencyLevel, // JLPT N5
	string Expression, // システム開発
	string Reading,
	string? AudioKey,
	bool IsActive,
	KnowledgeCollectionBasicDto KnowledgeCollection
	);

public sealed record KnowledgeCollectionBasicDto(
	Guid Id,
	string Scope,
	string Type,
	string Name,
	string? Description
	);

public sealed record KnowledgeMeaningDto(
	Guid Id,
	int SortOrder,
	string Meaning,
	string JapaneseMeaning,
	string? Description,
	int? AlternativeMeaningCount,
	IReadOnlyCollection<KnowledgeMeaningDto>? AlternativeMeanings
	);