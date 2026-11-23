using FluentValidation;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Application.Usecases.Curation.Learner.SubmitIssue;

public sealed class SubmitIssueValidator : AbstractValidator<SubmitIssueCommand>
{
	private static readonly HashSet<string> ValidTranscriptSegmentCategories = new(StringComparer.OrdinalIgnoreCase)
	{
		"AudioVideoSync",
		"CutoffContext",
		"GrammarOrTranscriptError",
		"SubtitleTranslationError",
		CurationContextHelper.OovCategory
	};

	private static readonly HashSet<string> ValidLearningUnitCategories = new(StringComparer.OrdinalIgnoreCase)
	{
		"IncorrectMatching"
	};

	private static readonly HashSet<string> ValidBankQuestionCategories = new(StringComparer.OrdinalIgnoreCase)
	{
		"WrongAnswerKey",
		"WrongExplanation",
		"DuplicateOptions",
		"TypoOrFormatError",
		"TypingEvaluationError"
	};

	public SubmitIssueValidator()
	{
		RuleFor(x => x.TargetType)
			.NotEmpty()
			.Must(t => string.Equals(t, nameof(TargetType.TranscriptSegment), StringComparison.OrdinalIgnoreCase)
			        || string.Equals(t, nameof(TargetType.LearningUnit), StringComparison.OrdinalIgnoreCase)
			        || string.Equals(t, nameof(TargetType.BankQuestion), StringComparison.OrdinalIgnoreCase))
			.WithMessage("TargetType is invalid. Allowed: TranscriptSegment, LearningUnit, BankQuestion.");

		RuleFor(x => x.TargetId)
			.NotEmpty()
			.WithMessage("TargetId cannot be empty.");

		RuleFor(x => x.Category)
			.NotEmpty()
			.MaximumLength(50)
			.WithMessage("Category cannot be empty or exceed 50 characters.")
			.Must((cmd, category) => IsCategoryValidForTargetType(cmd.TargetType, category))
			.WithMessage("Category is invalid for the specified TargetType.");

		RuleFor(x => x.LearnerFeedback)
			.MaximumLength(1000)
			.When(x => !string.IsNullOrEmpty(x.LearnerFeedback))
			.WithMessage("LearnerFeedback cannot exceed 1000 characters.");

		When(x => string.Equals(x.Category, CurationContextHelper.OovCategory, StringComparison.OrdinalIgnoreCase), () =>
		{
			RuleFor(x => x.StartCharIndex)
				.NotNull()
				.GreaterThanOrEqualTo(0)
				.WithMessage("StartCharIndex is required and must be >= 0 for OOV reports.");

			RuleFor(x => x.EndCharIndex)
				.NotNull()
				.GreaterThan(x => x.StartCharIndex ?? -1)
				.WithMessage("EndCharIndex is required and must be greater than StartCharIndex for OOV reports.");

			RuleFor(x => x)
				.Must(x => (x.EndCharIndex ?? 0) - (x.StartCharIndex ?? 0) <= 100)
				.WithMessage("Selected OOV span cannot exceed 100 characters.");
		});

		When(x => !string.Equals(x.Category, CurationContextHelper.OovCategory, StringComparison.OrdinalIgnoreCase), () =>
		{
			RuleFor(x => x.StartCharIndex)
				.Null()
				.WithMessage("StartCharIndex is only allowed for OOV reports.");

			RuleFor(x => x.EndCharIndex)
				.Null()
				.WithMessage("EndCharIndex is only allowed for OOV reports.");
		});
	}

	private static bool IsCategoryValidForTargetType(string? targetType, string? category)
	{
		if (string.IsNullOrWhiteSpace(category))
			return false;

		if (string.Equals(targetType, nameof(TargetType.TranscriptSegment), StringComparison.OrdinalIgnoreCase))
			return ValidTranscriptSegmentCategories.Contains(category);

		if (string.Equals(targetType, nameof(TargetType.LearningUnit), StringComparison.OrdinalIgnoreCase))
			return ValidLearningUnitCategories.Contains(category);

		if (string.Equals(targetType, nameof(TargetType.BankQuestion), StringComparison.OrdinalIgnoreCase))
			return ValidBankQuestionCategories.Contains(category);

		return false;
	}
}
