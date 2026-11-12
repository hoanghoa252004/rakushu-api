using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.ToggleBankQuestionStatus;

public sealed class ToggleBankQuestionStatusValidator : AbstractValidator<ToggleBankQuestionStatusCommand>
{
	public ToggleBankQuestionStatusValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Question ID cannot be empty.");
	}
}
