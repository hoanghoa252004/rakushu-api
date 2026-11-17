namespace Rakushu.Application.Abstractions.Infrastructure.Ai;

public interface IAiService
{
	Task<AiChatResult> ConverseAsync(
		AiChatRequest request,
		CancellationToken cancellationToken = default);

	IAsyncEnumerable<AiStreamChunk> ConverseStreamAsync(
		AiChatRequest request,
		CancellationToken cancellationToken = default);
}

public sealed record AiImage(byte[] Bytes, string ContentType);

public sealed record AiChatTurn(bool IsUser, string Text, AiImage? Image = null);

// History: earlier turns (oldest first). Question is the latest user turn.
// Contexts are passed to the managed Bedrock prompt as variables.
public sealed record AiRelatedMessage(string VideoTitle, bool IsUser, string Text);

// History: latest messages of the SAME conversation. RelatedMessages: similar messages from OTHER videos.
public sealed record AiChatRequest(
	IReadOnlyList<AiChatTurn> History,
	AiChatTurn Question,
	string VideoContext,
	string TranscriptContext,
	string LearningUnitsContext,
	IReadOnlyList<AiRelatedMessage>? RelatedMessages = null);

// Type: transcript_segment | learning_unit.
public sealed record AiReference(string Type, string Id, string Reason);

public sealed record AiChatResult(
	string Text,
	int? InputTokens,
	int? OutputTokens,
	bool Blocked,
	IReadOnlyList<AiReference>? References = null);

// Either a text delta, or the final chunk carrying usage / blocked flag.
public sealed record AiStreamChunk(
	string? TextDelta,
	bool IsFinal = false,
	int? InputTokens = null,
	int? OutputTokens = null,
	bool Blocked = false,
	IReadOnlyList<AiReference>? References = null);
