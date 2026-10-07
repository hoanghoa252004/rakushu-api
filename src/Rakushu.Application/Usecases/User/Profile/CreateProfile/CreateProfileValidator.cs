using FluentValidation;
using Rakushu.Application.Usecases.User.Profile.UpdateProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.User.Profile.CreateProfile;

internal sealed class CreateProfileValidator : AbstractValidator<CreateProfileCommand>
{
	public CreateProfileValidator()
	{
		RuleFor(x => x.LevelId)
			.NotEmpty().WithMessage("LevelId is required.");

		RuleFor(x => x.DailyLearningMinutes)
			.GreaterThanOrEqualTo(0).WithMessage("DailyLearningMinutes must be greater than or equal to 0.");

		RuleFor(x => x.SessionDurationMinutes)
			.GreaterThanOrEqualTo(0).WithMessage("SessionDurationMinutes must be greater than or equal to 0.");

		RuleForEach(x => x.Interests)
			.ChildRules(rules => {
				rules.RuleFor(x => x.ContentCategoryId)
					.NotEmpty().WithMessage("ContentCategoryId is required.");

				rules.RuleFor(x => x.Priority)
					.GreaterThan(0).WithMessage("Priority must be greater than 0.");
			})
			.When(x => x.Interests != null && x.Interests.Count > 0);
	}
}