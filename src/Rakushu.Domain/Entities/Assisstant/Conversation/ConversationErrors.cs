using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Assisstant.Conversation;

public static class ConversationErrors
{
	public static readonly Error UserNotFound = Error.Unauthorized(
		"ASSISTANT.USER_NOT_FOUND", "Current user could not be determined.");

	public static readonly Error SegmentNotFound = Error.NotFound(
		"ASSISTANT.SEGMENT_NOT_FOUND", "The transcript segment was not found in this video.");

	public static readonly Error LearningUnitNotFound = Error.NotFound(
		"ASSISTANT.LEARNING_UNIT_NOT_FOUND", "The learning unit was not found.");

	public static readonly Error EmptyContent = Error.Validation(
		"ASSISTANT.EMPTY_CONTENT", "Message content is required.");

	public static readonly Error InvalidAttachment = Error.Validation(
		"ASSISTANT.INVALID_ATTACHMENT", "Attachment must be a png, jpeg, gif or webp image up to 5 MB.");

	public static readonly Error AttachmentNotFound = Error.NotFound(
		"ASSISTANT.ATTACHMENT_NOT_FOUND", "The attachment file was not found in storage.");

	public static readonly Error ConversationNotFound = Error.NotFound(
		"ASSISTANT.CONVERSATION_NOT_FOUND", "The conversation was not found.");

	public static readonly Error AiFailed = Error.Failure(
		"ASSISTANT.AI_FAILED", "The assistant could not generate a response.");
}
