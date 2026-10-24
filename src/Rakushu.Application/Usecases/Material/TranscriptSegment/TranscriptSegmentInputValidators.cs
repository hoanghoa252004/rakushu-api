using FluentValidation;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment;

internal sealed class TranscriptItemInputValidator : AbstractValidator<TranscriptItemInput>
{
	public TranscriptItemInputValidator()
	{
		RuleFor(x => x.Text).NotEmpty().WithMessage("Item text is required");
		RuleFor(x => x.Type).IsInEnum().WithMessage("Item type is invalid");
		RuleFor(x => x.StartTime).GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("Item StartTime must be non-negative");
		RuleFor(x => x.EndTime).GreaterThanOrEqualTo(x => x.StartTime).WithMessage("Item EndTime must not be before StartTime");
		RuleFor(x => x.Confidence).InclusiveBetween(0m, 1m).WithMessage("Item confidence must be between 0 and 1");
		RuleFor(x => x.Sequence).GreaterThan(0).WithMessage("Item sequence must be greater than 0");
	}
}

internal sealed class SubtitleInputValidator : AbstractValidator<SubtitleInput>
{
	public SubtitleInputValidator()
	{
		RuleFor(x => x.Text).NotEmpty().WithMessage("Subtitle text is required");
		RuleFor(x => x.LanguageCode).IsInEnum().WithMessage("Subtitle language code is invalid");
	}
}
