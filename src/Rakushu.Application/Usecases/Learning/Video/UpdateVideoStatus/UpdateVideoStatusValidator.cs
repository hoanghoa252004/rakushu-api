using FluentValidation;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.UpdateVideoStatus;

internal sealed class UpdateVideoStatusValidator : AbstractValidator<UpdateVideoStatusCommand>
{
	public UpdateVideoStatusValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.NewStatus)
			.IsInEnum().WithMessage("Invalid video status");
	}
}
