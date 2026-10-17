using FluentValidation;

namespace Rakushu.Application.Usecases.Learning.Srs.EnrollLearningUnit;

internal sealed class EnrollLearningUnitValidator : AbstractValidator<EnrollLearningUnitCommand>
{
    public EnrollLearningUnitValidator()
    {
        RuleFor(x => x.LearningUnitId)
            .NotEmpty().WithMessage("LearningUnitId is required");
    }
}
