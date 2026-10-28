using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttemptValidator : AbstractValidator<RetryQuizAttemptCommand>
{
	public RetryQuizAttemptValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty()
			.WithMessage("Attempt ID is required.");

		RuleFor(x => x.RetryMode)
			.Must(m => m.Equals("All", StringComparison.OrdinalIgnoreCase) || m.Equals("MistakesOnly", StringComparison.OrdinalIgnoreCase))
			.WithMessage("Retry mode must be either 'All' or 'MistakesOnly'.");
	}
}
