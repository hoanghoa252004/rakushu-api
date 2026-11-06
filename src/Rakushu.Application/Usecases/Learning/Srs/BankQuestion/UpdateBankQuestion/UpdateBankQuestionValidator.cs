using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.UpdateBankQuestion;

public sealed class UpdateBankQuestionValidator : AbstractValidator<UpdateBankQuestionCommand>
{
	private static readonly string[] ValidDifficulties = ["Easy", "Medium", "Hard"];

	public UpdateBankQuestionValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Question ID cannot be empty.");

		RuleFor(x => x.Question)
			.NotEmpty()
			.MaximumLength(1000)
			.WithMessage("Question cannot be empty or exceed 1000 characters.");

		RuleFor(x => x.Answers)
			.NotNull()
			.Must(c => c != null && c.Count >= 2 && c.Count <= 6)
			.WithMessage("Answers must contain between 2 and 6 options.");

		RuleForEach(x => x.Answers)
			.ChildRules(choice =>
			{
				choice.RuleFor(c => c.Id).NotEmpty().WithMessage("Answer ID cannot be empty.");
				choice.RuleFor(c => c.Text).NotEmpty().WithMessage("Answer text cannot be empty.");
			});

		RuleFor(x => x.CorrectAnswer)
			.NotEmpty()
			.WithMessage("Correct answer cannot be empty.");

		RuleFor(x => x.Difficulty)
			.Must(d => ValidDifficulties.Contains(d, StringComparer.OrdinalIgnoreCase))
			.WithMessage("Difficulty must be Easy, Medium, or Hard.");
	}
}
