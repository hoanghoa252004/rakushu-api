using FluentValidation;

namespace Rakushu.Application.Usecases.Curator.Oov.ReviewOovCandidate;

internal sealed class ReviewOovCandidateValidator : AbstractValidator<ReviewOovCandidateCommand>
{
	public ReviewOovCandidateValidator()
	{
		RuleFor(x => x.OovCandidateId)
			.NotEmpty();

		RuleFor(x => x.CuratorId)
			.NotEmpty();

		RuleFor(x => x.Decision)
			.IsInEnum();
	}
}
