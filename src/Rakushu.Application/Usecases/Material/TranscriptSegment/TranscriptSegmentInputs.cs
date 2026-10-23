using Rakushu.Domain.Entities.Material.Subtitle;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment;

public sealed record TranscriptItemInput(
	string Text,
	TranscriptItemType Type,
	TimeSpan StartTime,
	TimeSpan EndTime,
	decimal Confidence,
	int Sequence);

public sealed record SubtitleInput(
	string Text,
	LanguageCode LanguageCode = LanguageCode.VN);
