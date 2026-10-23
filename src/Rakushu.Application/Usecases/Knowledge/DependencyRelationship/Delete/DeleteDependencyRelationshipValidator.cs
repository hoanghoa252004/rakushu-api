using FluentValidation;

namespace Rakushu.Application.Usecases.Knowledge.DependencyRelationship.Delete;

public sealed class DeleteDependencyRelationshipValidator : AbstractValidator<DeleteDependencyRelationshipCommand>
{
	public DeleteDependencyRelationshipValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("ID is required");
	}
}
