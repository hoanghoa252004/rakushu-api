using FluentValidation;

namespace Rakushu.Application.Usecases.Material.LearningUnit.AddLearningUnit;

internal sealed class AddLearningUnitValidator : AbstractValidator<AddLearningUnitCommand>
{
	public AddLearningUnitValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty().WithMessage("VideoId is required");
		RuleFor(x => x.SegmentId).NotEmpty().WithMessage("SegmentId is required");
		RuleFor(x => x.KnowledgeMeaningId).NotEmpty().WithMessage("KnowledgeMeaningId is required");
		RuleFor(x => x.StartTokenIndex).GreaterThanOrEqualTo(0).WithMessage("StartTokenIndex must be non-negative");
		RuleFor(x => x.EndTokenIndex)
			.GreaterThanOrEqualTo(x => x.StartTokenIndex).WithMessage("EndTokenIndex must not be less than StartTokenIndex");
	}
}
