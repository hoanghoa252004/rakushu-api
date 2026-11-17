using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Assisstant.Attachment;
using Rakushu.Domain.Entities.Assisstant.Message;
using Rakushu.Domain.Entities.Assisstant.Message.MessageEmbedding;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Assisstant.Conversation;

public interface IConversationRepository : IBaseRepository<Conversation, ConversationId>
{
	Task<Conversation?> GetByLearnerAndVideoAsync(
		UserId learnerId,
		VideoId videoId,
		CancellationToken cancellationToken = default);

	// Latest messages (with attachment), returned in ascending order of CreatedAt.
	Task<IReadOnlyList<Message.Message>> GetMessagesAsync(
		ConversationId conversationId,
		DateTime? before,
		int limit,
		CancellationToken cancellationToken = default);

	// Top-K messages of the conversation nearest to the vector (cosine distance), with attachment.
	// Only messages from the learner's OTHER conversations (other videos); Conversation and Video are loaded.
	Task<IReadOnlyList<Message.Message>> GetSimilarMessagesAsync(
		UserId learnerId,
		ConversationId currentConversationId,
		float[] vector,
		MessageId excludeMessageId,
		int top,
		CancellationToken cancellationToken = default);

	// Segment at the given id plus <radius> segments before and after (same transcript),
	// ordered by sequence, with TranscriptSpeaker and Speaker loaded.
	// Empty when the segment does not belong to the video.
	Task<IReadOnlyList<TranscriptSegment>> GetSegmentWindowAsync(
		VideoId videoId,
		TranscriptSegmentId segmentId,
		int radius,
		CancellationToken cancellationToken = default);

	// Learning unit with KnowledgeMeaning -> KnowledgeUnit -> all Meanings loaded (no tracking).
	Task<Material.LearningUnit.LearningUnit?> GetLearningUnitContextAsync(
		Material.LearningUnit.LearningUnitId learningUnitId,
		CancellationToken cancellationToken = default);

	Task<bool> VideoExistsAsync(VideoId videoId, CancellationToken cancellationToken = default);

	// Video with ContentCategory and Series loaded (no tracking).
	Task<Material.Video.Video?> GetVideoContextAsync(VideoId videoId, CancellationToken cancellationToken = default);

	Task<string?> GetConversationTitleAsync(UserId learnerId, VideoId videoId, CancellationToken cancellationToken = default);

	void AddMessage(Message.Message message);
	void AddAttachment(Attachment.Attachment attachment);
	void AddEmbedding(MessageEmbedding embedding);
}
