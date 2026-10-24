using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.UpdateSpeaker;


internal sealed class UpdateSpeakerValidator : AbstractValidator<UpdateSpeakerCommand>
{
	public UpdateSpeakerValidator()
	{
		RuleFor(x => x.SpeakerId).NotEmpty();
		RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
		RuleFor(x => x.JapaneseName).NotEmpty().MaximumLength(100);
		RuleFor(x => x.AvatarKey).MaximumLength(500);
		RuleFor(x => x.ThemeColor).NotEmpty().MaximumLength(10);
	}
}
