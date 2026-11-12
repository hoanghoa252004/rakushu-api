using FluentValidation;

namespace Rakushu.Application.Usecases.Curation.Curator.DismissReport;

public sealed class DismissReportValidator : AbstractValidator<DismissReportCommand>
{
	public DismissReportValidator()
	{
		RuleFor(x => x.ReportId)
			.NotEmpty()
			.WithMessage("ReportId cannot be empty.");

		RuleFor(x => x.Reason)
			.NotEmpty()
			.MaximumLength(2000)
			.WithMessage("Reason cannot be empty or exceed 2000 characters.");
	}
}
