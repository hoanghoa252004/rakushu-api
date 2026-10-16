using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Video.DeleteVideo;

internal sealed class DeleteVideoValidator : AbstractValidator<DeleteVideoCommand>
{
	public DeleteVideoValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");
	}
}
