namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetMeaningMediaContexts;

public sealed record MeaningMediaContextDto(
	Guid LearningUnitId,
	Guid TranscriptSegmentId,
	Guid VideoId,
	string VideoTitle,
	string? VideoUrl,
	string? StorageKey,
	double StartTime,
	double EndTime,
	string OriginalSentence,
	string ClozeSentence,
	string? TranslatedSentence,
	int StartTokenIndex,
	int EndTokenIndex);
