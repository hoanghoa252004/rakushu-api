using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetQuizDiagnostics;

internal sealed class GetQuizDiagnosticsValidator : AbstractValidator<GetQuizDiagnosticsQuery>
{
	public GetQuizDiagnosticsValidator()
	{
		When(x => x.Days.HasValue, () =>
		{
			RuleFor(x => x.Days!.Value)
				.InclusiveBetween(1, 365)
				.WithMessage("Days must be between 1 and 365.");
		});

		When(x => !string.IsNullOrWhiteSpace(x.QuestionTypeCode), () =>
		{
			RuleFor(x => x.QuestionTypeCode!)
				.MaximumLength(50)
				.WithMessage("QuestionTypeCode cannot exceed 50 characters.");
		});
	}
}
