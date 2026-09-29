using FluentValidation;

namespace Rakushu.Application.Usecases.Curator.Oov.ManualAddOovCandidate;

internal sealed class ManualAddOovCandidateValidator : AbstractValidator<ManualAddOovCandidateCommand>
{
	public ManualAddOovCandidateValidator()
	{
		RuleFor(x => x.OovCandidateId)
			.NotEmpty();

		RuleFor(x => x.CuratorId)
			.NotEmpty();

		RuleFor(x => x.Term)
			.NotEmpty()
			.MaximumLength(255);

		RuleFor(x => x.Reading)
			.NotEmpty()
			.MaximumLength(255);

		RuleFor(x => x.Meaning)
			.NotEmpty();
	}
}
