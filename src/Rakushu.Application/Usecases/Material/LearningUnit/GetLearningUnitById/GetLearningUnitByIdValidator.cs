using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitById;

internal sealed class GetLearningUnitByIdValidator : AbstractValidator<GetLearningUnitByIdQuery>
{
	public GetLearningUnitByIdValidator()
	{
		RuleFor(x => x.LearningUnitId).NotEmpty().WithMessage("LearningUnitId is required");
	}
}