using FluentValidation;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegment;

internal sealed class UpdateTranscriptSegmentValidator : AbstractValidator<UpdateTranscriptSegmentCommand>
{
	public UpdateTranscriptSegmentValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.SegmentId)
			.NotEmpty().WithMessage("SegmentId is required");

		RuleFor(x => x.Text)
			.NotEmpty().WithMessage("Text is required")
			.MaximumLength(5000).WithMessage("Text must not exceed 5000 characters");

		RuleFor(x => x.StartTime)
			.GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("StartTime must be non-negative")
			.LessThan(x => x.EndTime).WithMessage("StartTime must be less than EndTime");

		RuleFor(x => x.EndTime)
			.GreaterThan(TimeSpan.Zero).WithMessage("EndTime must be positive");

		RuleFor(x => x.SpeakerId)
			.NotEmpty().When(x => x.SpeakerId.HasValue).WithMessage("SpeakerId is invalid");

		RuleFor(x => x.Subtitle!)
			.SetValidator(new SubtitleInputValidator())
			.When(x => x.Subtitle is not null);

		RuleFor(x => x.Items)
			.NotNull().WithMessage("Items is required");

		RuleForEach(x => x.Items)
			.SetValidator(new TranscriptItemInputValidator());

		RuleFor(x => x.Sequence)
			.GreaterThan(0).When(x => x.Sequence.HasValue).WithMessage("Sequence must be greater than 0");
	}
}
