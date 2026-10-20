using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Nlp.NlpProcess;

// Transcript Analysis Response
public sealed class TranscriptNlpAnalysisResponse
{
	[JsonPropertyName("model")]
	public string Model { get; init; } = string.Empty;

	[JsonPropertyName("num_segments")]
	public int NumSegments { get; init; }

	[JsonPropertyName("results")]
	public List<TranscriptSegmentNlpResult> Results { get; init; } = [];
}

public sealed class TranscriptSegmentNlpResult
{
	[JsonPropertyName("id")]
	public Guid Id { get; init; }

	[JsonPropertyName("sequence")]
	public int Sequence { get; init; }

	[JsonPropertyName("text")]
	public string Text { get; init; } = string.Empty;

	[JsonPropertyName("tokens")]
	public List<TranscriptNlpTokenResult> Tokens { get; init; } = [];

	[JsonPropertyName("bunsetu")]
	public List<NlpBunsetsuResult> Bunsetsu { get; init; } = [];
}

public sealed class TranscriptNlpTokenResult
{
	[JsonPropertyName("i")]
	public int Index { get; init; }

	[JsonPropertyName("text")]
	public string Text { get; init; } = string.Empty;

	[JsonPropertyName("idx")]
	public int StartIndex { get; init; }

	[JsonPropertyName("lemma")]
	public string Lemma { get; init; } = string.Empty;

	[JsonPropertyName("reading")]
	public string Reading { get; init; } = string.Empty;

	[JsonPropertyName("tag")]
	public string Tag { get; init; } = string.Empty;

	[JsonPropertyName("dep")]
	public string Dependency { get; init; } = string.Empty;

	[JsonPropertyName("head")]
	public int Head { get; init; }
}

public sealed class NlpBunsetsuResult
{
	[JsonPropertyName("text")]
	public string Text { get; init; } = string.Empty;

	[JsonPropertyName("start_token")]
	public int StartToken { get; init; }

	[JsonPropertyName("end_token")]
	public int EndToken { get; init; }

	[JsonPropertyName("start_char")]
	public int StartIndex { get; init; }

	[JsonPropertyName("end_char")]
	public int EndIndex { get; init; }
}


// Knowledge Analysis Response
public sealed class KnowledgeNlpAnalysisResponse
{
	[JsonPropertyName("model")]
	public string Model { get; init; } = string.Empty;

	[JsonPropertyName("num_expressions")]
	public int NumExpressions { get; init; }

	[JsonPropertyName("results")]
	public List<KnowledgeUnitNlpTokenResult> Results { get; init; } = [];
}



public sealed class KnowledgeUnitNlpTokenResult
{
	[JsonPropertyName("id")]
	public Guid Id { get; init; }

	[JsonPropertyName("text")]
	public string Text { get; init; } = string.Empty;

	[JsonPropertyName("tokens")]
	public List<KnowledgeTokenNlpTokenResult> Tokens { get; init; } = [];
}

public sealed class KnowledgeTokenNlpTokenResult
{
	[JsonPropertyName("i")]
	public int Index { get; init; }

	[JsonPropertyName("text")]
	public string Text { get; init; } = string.Empty;

	[JsonPropertyName("idx")]
	public int StartIndex { get; init; }

	[JsonPropertyName("lemma")]
	public string Lemma { get; init; } = string.Empty;

	[JsonPropertyName("reading")]
	public string Reading { get; init; } = string.Empty;

	[JsonPropertyName("tag")]
	public string Tag { get; init; } = string.Empty;

	[JsonPropertyName("dep")]
	public string Dependency { get; init; } = string.Empty;

	[JsonPropertyName("head")]
	public int Head { get; init; }
}
