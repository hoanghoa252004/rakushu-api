using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Message;

public sealed partial class Message : Entity<MessageId>
{
	public ConversationId ConversationId { get; private set; } = null!;
	public TranscriptSegmentId TranscriptSegmentId { get; private set; } = null!;
	public Material.LearningUnit.LearningUnitId? LearningUnitId { get; private set; }
	public MessageRole Role { get; private set; }
	public string? Content { get; private set; }
	public MessageStatus Status { get; private set; }
	public int? InputTokens { get; private set; }
	public int? OutputTokens { get; private set; }
	public string? ErrorMessage { get; private set; }
	// Debug only: the payload (context + question) that was sent to the AI to produce this message.
	public string? AiRequestPayload { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime? CompletedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// Conversation
	public Conversation.Conversation Conversation { get; private set; } = null!;

	// TranscriptSegment
	public TranscriptSegment TranscriptSegment { get; private set; } = null!;


	// Attachment
	public Attachment.Attachment? Attachment { get; private set; }

	// Embedding
	public MessageEmbedding.MessageEmbedding? Embedding { get; private set; }

	// LearningUnit
	public Material.LearningUnit.LearningUnit? LearningUnit { get; private set; }

	private Message()
	{
	}

	private Message(
		MessageId id,
		ConversationId conversationId,
		TranscriptSegmentId transcriptSegmentId,
		MessageRole role,
		string? content,
		MessageStatus status)
		: base(id)
	{
		ConversationId = conversationId;
		TranscriptSegmentId = transcriptSegmentId;
		Role = role;
		Content = content;
		Status = status;
		CreatedAt = DateTime.UtcNow;
		if (status == MessageStatus.Completed)
			CompletedAt = CreatedAt;
	}

	public static Message CreateUserMessage(
		ConversationId conversationId,
		TranscriptSegmentId transcriptSegmentId,
		string content,
		Material.LearningUnit.LearningUnitId? learningUnitId = null)
	{
		var message = new Message(MessageId.Create(), conversationId, transcriptSegmentId, MessageRole.User, content, MessageStatus.Completed);
		message.LearningUnitId = learningUnitId;
		return message;
	}

	public static Message CreateAssistantPlaceholder(
		ConversationId conversationId,
		TranscriptSegmentId transcriptSegmentId)
		=> new(MessageId.Create(), conversationId, transcriptSegmentId, MessageRole.Assistant, null, MessageStatus.Processing);

	public void SetAiRequestPayload(string? payload) => AiRequestPayload = payload;

	public void Complete(string content, int? inputTokens, int? outputTokens)
	{
		Content = content;
		InputTokens = inputTokens;
		OutputTokens = outputTokens;
		Status = MessageStatus.Completed;
		CompletedAt = DateTime.UtcNow;
	}

	public void Fail(string errorMessage)
	{
		ErrorMessage = errorMessage.Length > 1000 ? errorMessage[..1000] : errorMessage;
		Status = MessageStatus.Failed;
		CompletedAt = DateTime.UtcNow;
	}
}