using FluentValidation;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Create;

public sealed class CreateDependencyRelationshipValidator : AbstractValidator<CreateDependencyRelationshipCommand>
{
	public CreateDependencyRelationshipValidator()
	{
		RuleFor(x => x.Code)
			.NotEmpty().WithMessage("Code is required.")
			.MaximumLength(50).WithMessage("Code cannot exceed 50 characters.");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Name is required.")
			.MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

		RuleFor(x => x.JapaneseName)
			.NotEmpty().WithMessage("JapaneseName is required.")
			.MaximumLength(200).WithMessage("JapaneseName cannot exceed 200 characters.");

		RuleFor(x => x.Description)
			.MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
	}
}
