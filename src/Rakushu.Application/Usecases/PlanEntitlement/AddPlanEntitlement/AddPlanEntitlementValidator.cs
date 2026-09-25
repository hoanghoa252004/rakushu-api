using FluentValidation;

namespace Rakushu.Application.Usecases.PlanEntitlement.AddPlanEntitlement;

internal sealed class AddPlanEntitlementValidator : AbstractValidator<AddPlanEntitlementCommand>
{
	public AddPlanEntitlementValidator()
	{
		RuleFor(x => x.PlanId)
			.NotEmpty()
			.WithMessage("Plan ID is required");

		RuleFor(x => x.FeatureId)
			.NotEmpty()
			.WithMessage("Feature ID is required");

		RuleFor(x => x.LimitValue)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Limit value must be greater than or equal to 0");

		RuleFor(x => x.LimitUnit)
			.NotEmpty()
			.WithMessage("Limit unit is required");

		RuleFor(x => x.LimitPeriod)
			.NotEmpty()
			.WithMessage("Limit period is required");
	}
}
