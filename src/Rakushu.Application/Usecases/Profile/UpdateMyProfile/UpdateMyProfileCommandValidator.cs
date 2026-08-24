using FluentValidation;

namespace Rakushu.Application.Usecases.Profile.UpdateMyProfile;

public sealed class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
	public UpdateMyProfileCommandValidator()
	{
		RuleFor(x => x.DisplayName)
			.NotEmpty().WithMessage("Display name is required.")
			.MaximumLength(100).WithMessage("Display name cannot exceed 100 characters.");

		RuleFor(x => x.AvatarUrl)
			.MaximumLength(500).WithMessage("Avatar URL cannot exceed 500 characters.");

		RuleFor(x => x.Bio)
			.MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.");

		RuleFor(x => x.NativeLanguage)
			.MaximumLength(50).WithMessage("Native language cannot exceed 50 characters.");

		RuleFor(x => x.LearningLanguage)
			.MaximumLength(50).WithMessage("Learning language cannot exceed 50 characters.");
	}
}
