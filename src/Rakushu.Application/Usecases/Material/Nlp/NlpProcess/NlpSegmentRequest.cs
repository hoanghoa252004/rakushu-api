using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Nlp.NlpProcess;


public sealed class TranscriptSegmentNlpRequest
{
	[JsonPropertyName("id")]
	public Guid Id { get; init; }

	[JsonPropertyName("sequence")]
	public int Sequence { get; init; }

	[JsonPropertyName("text")]
	public string Text { get; init; } = string.Empty;

	[JsonPropertyName("start_time")]
	public string StartTime { get; init; } = string.Empty;

	[JsonPropertyName("end_time")]
	public string EndTime { get; init; } = string.Empty;

	[JsonPropertyName("transcript_id")]
	public Guid TranscriptId { get; init; }

	[JsonPropertyName("transcript_speaker_id")]
	public Guid? TranscriptSpeakerId { get; init; }
}


public sealed class KnowledgeUnitNlpRequest
{
	[JsonPropertyName("id")]
	public Guid Id { get; init; }

	[JsonPropertyName("expression")]
	public string Expression { get; init; } = string.Empty;
}