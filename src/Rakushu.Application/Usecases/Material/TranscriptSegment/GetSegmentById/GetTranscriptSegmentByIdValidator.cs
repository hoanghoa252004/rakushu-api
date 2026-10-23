using FluentValidation;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.GetSegmentById;

internal sealed class GetTranscriptSegmentByIdValidator : AbstractValidator<GetTranscriptSegmentByIdQuery>
{
	public GetTranscriptSegmentByIdValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.SegmentId)
			.NotEmpty().WithMessage("SegmentId is required");
	}
}
