using FluentValidation;

namespace Rakushu.Application.Usecases.Material.Transcript.ImportOfficialTranscript;

internal sealed class ImportOfficialTranscriptValidator : AbstractValidator<ImportOfficialTranscriptCommand>
{
	public ImportOfficialTranscriptValidator()
	{
		RuleFor(x => x.VideoId)
			.NotEmpty().WithMessage("VideoId is required");

		RuleFor(x => x.FullText)
			.NotEmpty().WithMessage("FullText is required")
			.MaximumLength(50000).WithMessage("FullText must not exceed 50000 characters");

		RuleFor(x => x.StorageKey)
			.NotEmpty().WithMessage("StorageKey is required for Official transcript source");
	}
}
