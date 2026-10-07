using FluentValidation;
using Rakushu.Application.Usecases.Subscription.Feature.UpdateFeature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.UpdateProficiencyLevel;

internal sealed class UpdateProficiencyLevelValidator : AbstractValidator<UpdateProficiencyLevelCommand>
{
	public UpdateProficiencyLevelValidator()
	{
		RuleFor(x => x.ProficiencyLevelId)
			.NotEmpty()
			.WithMessage("Proficiency Level ID is required");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Proficiency Level name is required")
			.MaximumLength(100).WithMessage("Proficiency Level name must not exceed 100 characters");

		RuleFor(x => x.JapaneseName)
			.NotEmpty().WithMessage("Proficiency Level JapaneseName is required")
			.MaximumLength(100).WithMessage("Proficiency Level JapaneseName must not exceed 100 characters");

		RuleFor(x => x.SortOrder)
			.GreaterThan(0).WithMessage("Proficiency Level SortOrder must be greater than 0");

		RuleFor(x => x.IsActive)
			.NotNull().WithMessage("Proficiency Level IsActive is required");
	}
}
