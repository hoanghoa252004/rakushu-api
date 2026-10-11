using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.CreateSegment;

internal sealed class CreateTranscriptSegmentValidator : AbstractValidator<CreateTranscriptSegmentCommand>
{
	public CreateTranscriptSegmentValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.Text)
			.NotEmpty().WithMessage("Text is required")
			.MaximumLength(5000).WithMessage("Text must not exceed 5000 characters");

		RuleFor(x => x.StartTime)
			.GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("StartTime must be non-negative")
			.LessThan(x => x.EndTime).WithMessage("StartTime must be less than EndTime");

		RuleFor(x => x.EndTime)
			.GreaterThan(TimeSpan.Zero).WithMessage("EndTime must be positive");

		RuleFor(x => x.Sequence)
			.GreaterThan(0).When(x => x.Sequence.HasValue).WithMessage("Sequence must be greater than 0");
	}
}
