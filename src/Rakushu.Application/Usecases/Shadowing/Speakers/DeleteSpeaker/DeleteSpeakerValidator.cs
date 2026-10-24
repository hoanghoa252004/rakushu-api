using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.DeleteSpeaker;

internal sealed class DeleteSpeakerValidator : AbstractValidator<DeleteSpeakerCommand>
{
	public DeleteSpeakerValidator()
	{
		RuleFor(x => x.SpeakerId).NotEmpty();
	}
}