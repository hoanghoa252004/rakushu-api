using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;


public static class TranscriptSpeakerErrors
{
	public static readonly Error NotEmpty = Error.Validation(
		"TRANSCRIPT_SPEAKER.NOT_EMPTY", "The transcript speaker was not empty.");
}
