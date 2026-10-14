using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Video.UpdateVideoStatus;

internal sealed class ChangeVideoStatusValidator : AbstractValidator<ChangeVideoStatusCommand>
{
	public ChangeVideoStatusValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.NewStatus)
			.IsInEnum().WithMessage("Invalid video status");
	}
}
