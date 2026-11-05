using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

namespace Rakushu.Domain.Entities.Material.Subtitle;

public sealed class Subtitle : Entity<SubtitleId>
{
	public int Sequence { get; private set; }
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public LanguageCode LanguageCode { get; private set; }
	public string Text { get; private set; } = null!;

	// NAVIGATION PROPERTIES

	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;


	// CONSTRUCTORS & FACTORY METHODS

	private Subtitle()
	{
	}

	private Subtitle(
		SubtitleId id,
		TranscriptSegmentId transcriptSegmentId,
		LanguageCode languageCode,
		string text,
		int sequence)
		: base(id)
	{
		TranscriptSegmentId = transcriptSegmentId;
		LanguageCode = languageCode;
		Text = text;
		Sequence = sequence;
	}

	internal void SetSequence(int sequence)
	{
		Sequence = sequence;
	}

	public void Update(LanguageCode languageCode, string text, int sequence)
	{
		LanguageCode = languageCode;
		Text = text;
		Sequence = sequence;
	}

	public static Result<Subtitle> Create(
		TranscriptSegmentId transcriptSegmentId,
		LanguageCode languageCode,
		string text,
		int sequence)
	{
		return Result.Success(new Subtitle(
			SubtitleId.Create(),
			transcriptSegmentId,
			languageCode,
			text,
			sequence));
	}
}