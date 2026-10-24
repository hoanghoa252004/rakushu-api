using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakerById;

internal sealed class GetSpeakerByIdValidator : AbstractValidator<GetSpeakerByIdQuery>
{
	public GetSpeakerByIdValidator()
	{
		RuleFor(x => x.SpeakerId).NotEmpty().WithMessage("SpeakerId is required");
	}
}