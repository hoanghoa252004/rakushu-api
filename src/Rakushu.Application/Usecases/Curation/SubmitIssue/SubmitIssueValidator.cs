using FluentValidation;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Application.Usecases.Curation.SubmitIssue;

public sealed class SubmitIssueValidator : AbstractValidator<SubmitIssueCommand>
{
	public SubmitIssueValidator()
	{
		RuleFor(x => x.TargetType)
			.NotEmpty()
			.Must(t => Enum.TryParse<TargetType>(t, true, out _))
			.WithMessage("TargetType is invalid. Allowed: LearningUnit, KnowledgeMeaning, BankQuestion.");

		RuleFor(x => x.TargetId)
			.NotEmpty()
			.WithMessage("TargetId cannot be empty.");

		RuleFor(x => x.Category)
			.NotEmpty()
			.MaximumLength(100)
			.WithMessage("Category cannot be empty or exceed 100 characters.");

		RuleFor(x => x.LearnerFeedback)
			.MaximumLength(1000)
			.When(x => !string.IsNullOrEmpty(x.LearnerFeedback))
			.WithMessage("LearnerFeedback cannot exceed 1000 characters.");
	}
}
