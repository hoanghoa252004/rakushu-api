using FluentValidation;

namespace Rakushu.Application.Usecases.PlanEntitlement.UpdatePlanEntitlement;

internal sealed class UpdatePlanEntitlementValidator : AbstractValidator<UpdatePlanEntitlementCommand>
{
	public UpdatePlanEntitlementValidator()
	{
		RuleFor(x => x.PlanId)
			.NotEmpty()
			.WithMessage("Plan ID is required");

		RuleFor(x => x.EntitlementId)
			.NotEmpty()
			.WithMessage("Entitlement ID is required");

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
