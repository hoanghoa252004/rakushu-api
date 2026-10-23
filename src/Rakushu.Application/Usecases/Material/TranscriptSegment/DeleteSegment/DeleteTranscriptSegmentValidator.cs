using FluentValidation;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.DeleteSegment;

internal sealed class DeleteTranscriptSegmentValidator : AbstractValidator<DeleteTranscriptSegmentCommand>
{
	public DeleteTranscriptSegmentValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.SegmentId)
			.NotEmpty().WithMessage("SegmentId is required");
	}
}
