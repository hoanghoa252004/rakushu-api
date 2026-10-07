using FluentValidation;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.CreateEntitlement;

internal sealed class CreateEntitlementValidator : AbstractValidator<CreateEntitlementCommand>
{
	public CreateEntitlementValidator()
	{
		RuleFor(x => x.PlanId)
			.NotEmpty()
			.WithMessage("Plan ID is required");

		RuleFor(x => x.FeatureId)
			.NotEmpty()
			.WithMessage("Feature ID is required");

		RuleFor(x => x.LimitValue)
			.GreaterThan(0)
			.WithMessage("Limit value must be greater than 0");

		RuleFor(x => x.LimitUnit)
			.NotEmpty()
			.WithMessage("Limit unit is required");

		RuleFor(x => x.LimitPeriod)
			.NotEmpty()
			.WithMessage("Limit period is required");

		RuleFor(x => x.IsEnabled)
			.NotNull().WithMessage("Entitlement IsEnabled is required");
	}
}
