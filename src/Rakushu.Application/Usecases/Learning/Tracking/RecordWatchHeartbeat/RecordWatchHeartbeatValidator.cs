using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Tracking.RecordWatchHeartbeat;

internal sealed class RecordWatchHeartbeatValidator : AbstractValidator<RecordWatchHeartbeatCommand>
{
	public RecordWatchHeartbeatValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty()
			.WithMessage("VideoId cannot be empty.");

		RuleFor(x => x.CurrentPositionSeconds)
			.GreaterThanOrEqualTo(0)
			.WithMessage("CurrentPositionSeconds must be non-negative.");

		When(x => x.WatchedRange is not null, () =>
		{
			RuleFor(x => x.WatchedRange!.StartSeconds)
				.GreaterThanOrEqualTo(0)
				.WithMessage("WatchedRange.StartSeconds must be non-negative.");

			RuleFor(x => x.WatchedRange!.EndSeconds)
				.GreaterThanOrEqualTo(x => x.WatchedRange!.StartSeconds)
				.WithMessage("WatchedRange.EndSeconds must not be less than StartSeconds.");
		});

		RuleFor(x => x.PlaybackRate)
			.InclusiveBetween(0.25, 4.0)
			.WithMessage("PlaybackRate must be between 0.25 and 4.0.");
	}
}
