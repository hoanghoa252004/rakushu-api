using FluentValidation;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Delete;

public sealed class DeleteJapaneseConjugationFormValidator : AbstractValidator<DeleteJapaneseConjugationFormCommand>
{
	public DeleteJapaneseConjugationFormValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID is required");
	}
}
