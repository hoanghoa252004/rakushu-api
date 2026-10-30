using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.CreateBankQuestion;

public sealed class CreateBankQuestionValidator : AbstractValidator<CreateBankQuestionCommand>
{
	private static readonly string[] ValidDifficulties = ["Easy", "Medium", "Hard"];

	public CreateBankQuestionValidator()
	{
		RuleFor(x => x.KnowledgeMeaningId)
			.NotEmpty()
			.WithMessage("Knowledge meaning ID cannot be empty.");

		RuleFor(x => x.QuestionTypeId)
			.NotEmpty()
			.WithMessage("Question type ID cannot be empty.");

		RuleFor(x => x.Prompt)
			.NotEmpty()
			.MaximumLength(1000)
			.WithMessage("Prompt cannot be empty or exceed 1000 characters.");

		RuleFor(x => x.Choices)
			.NotNull()
			.Must(c => c != null && c.Count >= 2 && c.Count <= 6)
			.WithMessage("Choices must contain between 2 and 6 options.");

		RuleForEach(x => x.Choices)
			.ChildRules(choice =>
			{
				choice.RuleFor(c => c.Id).NotEmpty().WithMessage("Choice ID cannot be empty.");
				choice.RuleFor(c => c.Text).NotEmpty().WithMessage("Choice text cannot be empty.");
			});

		RuleFor(x => x.CorrectChoiceId)
			.NotEmpty()
			.WithMessage("Correct choice ID cannot be empty.");

		RuleFor(x => x.Difficulty)
			.Must(d => ValidDifficulties.Contains(d, StringComparer.OrdinalIgnoreCase))
			.WithMessage("Difficulty must be Easy, Medium, or Hard.");
	}
}
