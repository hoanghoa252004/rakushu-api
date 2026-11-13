using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.DeleteQuestion;

internal sealed class DeleteQuestionValidator : AbstractValidator<DeleteQuestionCommand>
{
	public DeleteQuestionValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Question ID cannot be empty.");
	}
}
