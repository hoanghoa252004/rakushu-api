using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Transcript.GetTranscript;

internal sealed class GetDetailTranscriptValidator : AbstractValidator<GetDetailTranscriptQuery>
{
	public GetDetailTranscriptValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.PageNumber)
			.GreaterThan(0).WithMessage("PageNumber must be greater than 0");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("PageSize must be greater than 0")
			.LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
	}
}
