using FluentValidation;

namespace Rakushu.Application.Usecases.Material.LearningUnit.RemoveLearningUnit;

internal sealed class RemoveLearningUnitValidator : AbstractValidator<RemoveLearningUnitCommand>
{
	public RemoveLearningUnitValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty().WithMessage("VideoId is required");
		RuleFor(x => x.SegmentId).NotEmpty().WithMessage("SegmentId is required");
		RuleFor(x => x.LearningUnitId).NotEmpty().WithMessage("LearningUnitId is required");
	}
}
