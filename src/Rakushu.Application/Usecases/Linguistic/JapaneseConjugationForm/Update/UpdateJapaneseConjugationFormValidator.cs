using FluentValidation;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Update;

public sealed class UpdateJapaneseConjugationFormValidator : AbstractValidator<UpdateJapaneseConjugationFormCommand>
{
	public UpdateJapaneseConjugationFormValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID is required");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Name is required")
			.MaximumLength(200)
			.WithMessage("Name must not exceed 200 characters");

		RuleFor(x => x.JapaneseName)
			.NotEmpty()
			.WithMessage("Japanese name is required")
			.MaximumLength(200)
			.WithMessage("Japanese name must not exceed 200 characters");

		RuleFor(x => x.Description)
			.MaximumLength(500)
			.WithMessage("Description must not exceed 500 characters")
			.When(x => x.Description != null);
	}
}
