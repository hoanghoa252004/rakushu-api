using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.BankQuestion.UpdateBankQuestion;

public sealed class UpdateBankQuestionValidator : AbstractValidator<UpdateBankQuestionCommand>
{
	private static readonly string[] ValidMediaTypes = ["NONE", "VIDEO", "AUDIO"];

	public UpdateBankQuestionValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Question ID cannot be empty.");

		RuleFor(x => x.Question)
			.NotEmpty()
			.MaximumLength(1000)
			.WithMessage("Question cannot be empty or exceed 1000 characters.");

		When(x => x.Answers != null && x.Answers.Count > 0, () =>
		{
			RuleFor(x => x.Answers!)
				.Must(c => c.Count >= 2 && c.Count <= 6)
				.WithMessage("Answers must contain between 2 and 6 options.")
				.Must(c => c.Select(a => a.Id.Trim().ToLowerInvariant()).Distinct().Count() == c.Count)
				.WithMessage("Answer IDs must be unique.")
				.Must(c => c.Select(a => a.Text.Trim().ToLowerInvariant()).Distinct().Count() == c.Count)
				.WithMessage("Answer choices cannot have identical text.");

			RuleForEach(x => x.Answers!)
				.ChildRules(choice =>
				{
					choice.RuleFor(c => c.Id).NotEmpty().WithMessage("Answer ID cannot be empty.");
					choice.RuleFor(c => c.Text).NotEmpty().WithMessage("Answer text cannot be empty.");
				});
		});

		RuleFor(x => x.CorrectAnswer)
			.NotEmpty()
			.WithMessage("Correct answer cannot be empty.");

		RuleFor(x => x.MediaType)
			.Must(m => string.IsNullOrWhiteSpace(m) || ValidMediaTypes.Contains(m.Trim(), StringComparer.OrdinalIgnoreCase))
			.WithMessage("MediaType must be NONE, VIDEO, AUDIO, or IMAGE.");
	}
}