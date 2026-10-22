using FluentValidation;
using Rakushu.Application.Usecases.Material.Transcript.GetTranscript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Transcript.GetLearningTranscript;

internal sealed class GetLearningTranscriptValidator : AbstractValidator<GetLearningTranscriptQuery>
{
	public GetLearningTranscriptValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.PageNumber)
			.GreaterThan(0).WithMessage("PageNumber must be greater than 0");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("PageSize must be greater than 0")
			.LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");
	}
}
