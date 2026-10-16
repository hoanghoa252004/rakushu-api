using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Video.ChangeVideoStatus;

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
