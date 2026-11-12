namespace Rakushu.Application.Abstractions.Infrastructure.Assessment;

public sealed record SpeechAssessmentResult(decimal Score, string RawResponse);

public interface ISpeechAssessmentService
{
	// Azure Speech pronunciation assessment (0 - 100).
	Task<SpeechAssessmentResult> AssessAccuracyAsync(
		string referenceText,
		string userAudioUrl,
		CancellationToken cancellationToken);

	// Local pitch service /compare (0 - 100).
	Task<SpeechAssessmentResult> AssessPitchSimilarityAsync(
		string referenceAudioUrl,
		string userAudioUrl,
		CancellationToken cancellationToken);
}
