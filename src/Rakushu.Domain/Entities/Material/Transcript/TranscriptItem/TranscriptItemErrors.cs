using Rakushu.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;

public static class TranscriptItemErrors
{
	public static readonly Error TextNotBeEmpty = Error.Validation(
		"TRANSCRIPT_ITEM.TEXT_NOT_BE_EMPTY", "The transcript item text cannot be empty.");

	public static readonly Error InvalidTimeRange = Error.Validation(
	"TRANSCRIPT_SPEAKER.INVALID_TIME_RANGE", "The transcript speaker has an invalid time range.");

	public static readonly Error InvalidSequence = Error.Validation(
		"TRANSCRIPT_SPEAKER.INVALID_SEQUENCE", "The transcript speaker has an invalid sequence.");
}