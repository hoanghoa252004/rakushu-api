using FluentValidation;

namespace Rakushu.Application.Usecases.User.Role.UpdateRole;

public sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
	public UpdateRoleValidator()
	{
		RuleFor(x => x.RoleId)
			.NotEmpty()
			.WithMessage("Role ID is required.");

		RuleFor(x => x.Description)
			.MaximumLength(500)
			.WithMessage("Description must not exceed 500 characters.")
			.When(x => x.Description != null);
	}
}
