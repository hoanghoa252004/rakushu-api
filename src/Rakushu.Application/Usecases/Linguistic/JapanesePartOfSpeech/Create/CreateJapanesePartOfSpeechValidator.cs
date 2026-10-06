using FluentValidation;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Create;

public sealed class CreateJapanesePartOfSpeechValidator : AbstractValidator<CreateJapanesePartOfSpeechCommand>
{
	public CreateJapanesePartOfSpeechValidator()
	{
		RuleFor(x => x.Code)
			.NotEmpty()
			.WithMessage("Code is required")
			.MaximumLength(100)
			.WithMessage("Code must not exceed 100 characters");

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
