using FluentValidation;

namespace Rakushu.Application.Usecases.Curator.Oov.GetOovCandidates;

internal sealed class GetOovCandidatesValidator : AbstractValidator<GetOovCandidatesQuery>
{
	public GetOovCandidatesValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThan(0);

		RuleFor(x => x.PageSize)
			.GreaterThan(0)
			.LessThanOrEqualTo(100);
	}
}
