using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Transcript.DeleteTranscript;

internal sealed class DeleteTranscriptValidator : AbstractValidator<DeleteTranscriptCommand>
{
	public DeleteTranscriptValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");
	}
}
