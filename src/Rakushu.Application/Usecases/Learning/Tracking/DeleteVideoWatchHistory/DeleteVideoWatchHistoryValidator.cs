using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Tracking.DeleteVideoWatchHistory;

internal sealed class DeleteVideoWatchHistoryValidator : AbstractValidator<DeleteVideoWatchHistoryCommand>
{
	public DeleteVideoWatchHistoryValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty()
			.WithMessage("VideoId cannot be empty.");
	}
}
