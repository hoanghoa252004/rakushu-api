using FluentValidation;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegmentSpeaker;

internal sealed class UpdateTranscriptSegmentSpeakerValidator
	: AbstractValidator<UpdateTranscriptSegmentSpeakerCommand>
{
	public UpdateTranscriptSegmentSpeakerValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.SegmentId)
			.NotEmpty().WithMessage("SegmentId is required");

		RuleFor(x => x.SpeakerId)
			.NotEmpty().When(x => x.SpeakerId.HasValue).WithMessage("SpeakerId is invalid");

		RuleFor(x => x.TranscriptSpeakerId)
			.NotEmpty().When(x => x.TranscriptSpeakerId.HasValue).WithMessage("TranscriptSpeakerId is invalid");

		RuleFor(x => x)
			.Must(x => !(x.SpeakerId.HasValue && x.TranscriptSpeakerId.HasValue))
			.WithMessage("Provide either SpeakerId or TranscriptSpeakerId, not both");
	}
}
