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

	public static readonly Error NotFound = Error.NotFound(
		"TRANSCRIPT_SPEAKER.NOT_FOUND", "The transcript speaker was not found.");

	public static readonly Error SpeakerAlreadyAssigned = Error.Conflict(
		"TRANSCRIPT_SPEAKER.SPEAKER_ALREADY_ASSIGNED",
		"This speaker is already assigned to a transcript speaker of this transcript.");
}
