using FluentValidation;

namespace Rakushu.Application.Usecases.Curation.DeleteMyIssue;

public sealed class DeleteMyIssueValidator : AbstractValidator<DeleteMyIssueCommand>
{
	public DeleteMyIssueValidator()
	{
		RuleFor(x => x.IssueId)
			.NotEmpty()
			.WithMessage("IssueId cannot be empty.");
	}
}
