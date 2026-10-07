using FluentValidation;

namespace Rakushu.Application.Usecases.Subscription.Plan.UpdatePlan;

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
			.MaximumLength(100)
			.WithMessage("Plan name must not exceed 100 characters");

		RuleFor(x => x.JapaneseName)
			.NotEmpty()
			.WithMessage("Plan JapaneseName is required")
			.MaximumLength(100)
			.WithMessage("Plan JapaneseName must not exceed 100 characters");

		RuleFor(x => x.Price)
			.GreaterThanOrEqualTo(0)
			.WithMessage("Price must be greater than or equal to 0");

		RuleFor(x => x.Currency)
			.NotEmpty()
			.WithMessage("Currency is required");

		RuleFor(x => x.BillingCycle)
			.NotEmpty()
			.WithMessage("Billing cycle is required");

		RuleFor(x => x.IsActive)
			.NotNull().WithMessage("Plan IsActive is required");
	}
}
