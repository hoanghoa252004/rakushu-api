using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.CreateSpeaker;

internal sealed class CreateSpeakerValidator : AbstractValidator<CreateSpeakerCommand>
{
	public CreateSpeakerValidator()
	{
		RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
		RuleFor(x => x.JapaneseName).NotEmpty().MaximumLength(100);
		RuleFor(x => x.AvatarKey).MaximumLength(500);
		RuleFor(x => x.ThemeColor).NotEmpty().MaximumLength(10);
	}
}