namespace Rakushu.Domain.Entities.Processing.ProcessingJobLog;

public enum ProcessingJobType
{
	YoutubeImport = 1,
	LanguageModeration = 2,
	ContentModeration = 3,
	Transcription = 4,
	Nlp = 5,
	Translation = 6,
	LearningUnitMatching = 7,
	ShadowingAudioClip = 8,
	ShadowingAudioNormalization = 9,
	PronunciationAssessment = 10,
	PitchAssessment = 11
}
