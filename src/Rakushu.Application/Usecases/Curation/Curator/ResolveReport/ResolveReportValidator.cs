using FluentValidation;

namespace Rakushu.Application.Usecases.Curation.Curator.ResolveReport;

public sealed class ResolveReportValidator : AbstractValidator<ResolveReportCommand>
{
	public ResolveReportValidator()
	{
		RuleFor(x => x.ReportId)
			.NotEmpty()
			.WithMessage("ReportId cannot be empty.");

		RuleFor(x => x.CuratorNotes)
			.NotEmpty()
			.MaximumLength(2000)
			.WithMessage("CuratorNotes cannot be empty or exceed 2000 characters.");
	}
}
