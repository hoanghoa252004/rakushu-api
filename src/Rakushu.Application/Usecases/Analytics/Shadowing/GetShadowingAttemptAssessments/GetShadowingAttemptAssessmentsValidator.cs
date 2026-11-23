using FluentValidation;

namespace Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;

internal sealed class GetShadowingAttemptAssessmentsValidator : AbstractValidator<GetShadowingAttemptAssessmentsQuery>
{
	public GetShadowingAttemptAssessmentsValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.AttemptId).NotEmpty();
	}
}
