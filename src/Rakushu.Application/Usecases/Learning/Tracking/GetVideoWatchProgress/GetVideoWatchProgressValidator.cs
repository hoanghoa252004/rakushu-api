using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVideoWatchProgress;

internal sealed class GetVideoWatchProgressValidator : AbstractValidator<GetVideoWatchProgressQuery>
{
	public GetVideoWatchProgressValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty()
			.WithMessage("VideoId cannot be empty.");
	}
}
