using Rakushu.Application.Usecases.Material.Transcript;

namespace Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitCurationContext;

public sealed record LearningUnitCurationContextDto(
	Guid LearningUnitId,
	Guid VideoId,
	string VideoTitle,
	string? VideoUrl,
	Guid SegmentId,
	int SegmentSequence,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string OriginalSentence,
	SubtitleDetailDto? Subtitle,
	int StartTokenIndex,
	int EndTokenIndex,
	Guid KnowledgeMeaningId,
	string Expression,
	string? Reading,
	string Meaning,
	IReadOnlyList<CurationTokenDto> Tokens,
	IReadOnlyList<SegmentLearningUnitDto> SegmentLearningUnits
);

public sealed record CurationTokenDto(
	Guid Id,
	int Sequence,
	int StartIndex,
	int EndIndex,
	string Surface,
	string Lemma,
	string Reading,
	string? PartOfSpeech
);

public sealed record SegmentLearningUnitDto(
	Guid Id,
	Guid KnowledgeMeaningId,
	string Expression,
	string Meaning,
	int StartTokenIndex,
	int EndTokenIndex,
	bool IsTarget
);
