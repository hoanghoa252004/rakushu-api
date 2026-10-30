using FluentValidation;

namespace Rakushu.Application.Usecases.Curation.Curator.MarkReportInReview;

public sealed class MarkReportInReviewValidator : AbstractValidator<MarkReportInReviewCommand>
{
	public MarkReportInReviewValidator()
	{
		RuleFor(x => x.ReportId)
			.NotEmpty()
			.WithMessage("ReportId cannot be empty.");
	}
}
