using FluentValidation;

namespace Rakushu.Application.Usecases.Profile.UpdateProfile;

internal sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
	public UpdateProfileValidator()
	{
		RuleFor(x => x.FullName)
			.NotEmpty().WithMessage("FullName is required.")
			.MaximumLength(50).WithMessage("FullName cannot exceed 50 characters.");

		RuleFor(x => x.NativeLanguage)
			.NotEmpty().WithMessage("NativeLanguage is required.")
			.MaximumLength(50).WithMessage("NativeLanguage cannot exceed 50 characters.");
	}
}
