using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttemptValidator : AbstractValidator<RetryQuizAttemptCommand>
{
	public RetryQuizAttemptValidator()
	{
		RuleFor(x => x.AttemptId)
			.NotEmpty().WithMessage("AttemptId is required.");

		RuleFor(x => x.RetryMode)
			.NotEmpty().WithMessage("RetryMode is required.")
			.Must(mode => string.Equals(mode, "All", StringComparison.OrdinalIgnoreCase) ||
			              string.Equals(mode, "MistakesOnly", StringComparison.OrdinalIgnoreCase))
			.WithMessage("Invalid retry mode. Supported modes are 'All' and 'MistakesOnly'.");
	}
}
