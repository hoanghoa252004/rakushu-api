using FluentValidation;

namespace Rakushu.Application.Usecases.Plan.UpdatePlan;

public sealed class UpdatePlanValidator : AbstractValidator<UpdatePlanCommand>
{
	public UpdatePlanValidator()
	{
		RuleFor(x => x.PlanId)
			.NotEmpty()
			.WithMessage("Plan ID is required");

		RuleFor(x => x.Name)
			.NotEmpty()
			.WithMessage("Plan name is required")
			.MaximumLength(50)
			.WithMessage("Plan name must not exceed 50 characters");

		RuleFor(x => x.Price)
			.GreaterThan(0)
			.WithMessage("Price must be greater than or equal to 0");

		RuleFor(x => x.Currency)
			.NotEmpty()
			.WithMessage("Currency is required");

		RuleFor(x => x.BillingCycle)
			.NotEmpty()
			.WithMessage("Billing cycle is required");
	}
}
