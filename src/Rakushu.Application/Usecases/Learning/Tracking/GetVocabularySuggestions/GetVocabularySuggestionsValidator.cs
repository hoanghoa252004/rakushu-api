using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVocabularySuggestions;

internal sealed class GetVocabularySuggestionsValidator : AbstractValidator<GetVocabularySuggestionsQuery>
{
	public GetVocabularySuggestionsValidator()
	{
		RuleFor(x => x.Limit)
			.InclusiveBetween(1, 50)
			.WithMessage("Limit must be between 1 and 50.");
	}
}
