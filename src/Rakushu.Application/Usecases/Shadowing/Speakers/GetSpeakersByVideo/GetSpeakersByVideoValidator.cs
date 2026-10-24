using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakersByVideo;

internal sealed class GetSpeakersByVideoValidator : AbstractValidator<GetSpeakersByVideoQuery>
{
	public GetSpeakersByVideoValidator()
	{
		RuleFor(x => x.VideoId).NotEmpty().WithMessage("VideoId is required");
	}
}