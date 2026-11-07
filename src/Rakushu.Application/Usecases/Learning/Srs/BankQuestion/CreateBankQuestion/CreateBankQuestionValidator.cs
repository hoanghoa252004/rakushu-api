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

		RuleFor(x => x.Question)
			.NotEmpty()
			.MaximumLength(1000)
			.WithMessage("Question cannot be empty or exceed 1000 characters.");

		RuleFor(x => x.Answers)
			.NotNull()
			.Must(c => c != null && c.Count >= 2 && c.Count <= 6)
			.WithMessage("Answers must contain between 2 and 6 options.")
			.Must(c => c == null || c.Select(a => a.Id.Trim().ToLowerInvariant()).Distinct().Count() == c.Count)
			.WithMessage("Answer IDs must be unique.")
			.Must(c => c == null || c.Select(a => a.Text.Trim().ToLowerInvariant()).Distinct().Count() == c.Count)
			.WithMessage("Answer choices cannot have identical text.");

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
