using Rakushu.Application.Abstractions.Infrastructure.Ai;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Rag;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Assisstant.Attachment;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Assisstant.Message;
using Rakushu.Domain.Entities.Assisstant.Message.MessageEmbedding;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using System.Text;

namespace Rakushu.Application.Usecases.Assistant;

public sealed record AttachmentInput(string FileKey, string ContentType, long FileSize, TimeSpan? Timestamp);

public sealed record PreparedChat(
	Conversation Conversation,
	Message UserMessage,
	TranscriptSegmentId SegmentId,
	AiChatRequest AiRequest,
	IReadOnlySet<string> ValidReferenceIds);

public sealed record ReferenceDto(string Type, string Id, string Reason);

public sealed record AssistantReplyDto(
	MessageDto UserMessage,
	MessageDto AssistantMessage,
	IReadOnlyList<ReferenceDto> References);

public sealed record AttachmentDto(string Url, string ContentType, long FileSize, TimeSpan Timestamp);

public sealed record MessageDto(
	Guid Id,
	Guid TranscriptSegmentId,
	Guid? LearningUnitId,
	string Role,
	string? Content,
	string Status,
	string? ErrorMessage,
	DateTime CreatedAt,
	DateTime? CompletedAt,
	AttachmentDto? Attachment);

// Shared by the Converse and ConverseStream flows.
public sealed class AssistantChatService(
	ICurrentUserContext currentUser,
	IConversationRepository conversationRepository,
	IEmbeddingService embeddingService,
	IStorageService storageService,
	IUnitOfWork unitOfWork)
{
	public const int RecentMessageCount = 10;
	public const int SimilarMessageCount = 5;
	public const int SegmentRadius = 2;
	public const long MaxImageBytes = 5 * 1024 * 1024;
	public const string BlockedReply =
		"Sorry, I can only help with questions about learning Japanese through videos.";

	private static readonly string[] AllowedImageTypes =
		["image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp"];

	public async Task<Result<PreparedChat>> PrepareAsync(
		Guid videoGuid,
		Guid segmentGuid,
		string content,
		AttachmentInput? attachment,
		Guid? learningUnitGuid,
		CancellationToken cancellationToken)
	{
		var learnerId = currentUser.UserId;
		if (learnerId is null)
			return Result.Failure<PreparedChat>(ConversationErrors.UserNotFound);

		content = content?.Trim() ?? string.Empty;
		if (content.Length == 0)
			return Result.Failure<PreparedChat>(ConversationErrors.EmptyContent);

		if (attachment is not null &&
			(!AllowedImageTypes.Contains(attachment.ContentType.ToLowerInvariant())
				|| attachment.FileSize <= 0 || attachment.FileSize > MaxImageBytes
				|| string.IsNullOrWhiteSpace(attachment.FileKey)))
			return Result.Failure<PreparedChat>(ConversationErrors.InvalidAttachment);

		var videoId = VideoId.From(videoGuid);
		var segmentId = TranscriptSegmentId.From(segmentGuid);

		var window = await conversationRepository.GetSegmentWindowAsync(
			videoId, segmentId, SegmentRadius, cancellationToken);
		if (window.Count == 0)
			return Result.Failure<PreparedChat>(ConversationErrors.SegmentNotFound);

		var learningUnitId = learningUnitGuid.HasValue
			? LearningUnitId.From(learningUnitGuid.Value)
			: null;
		LearningUnit? learningUnit = null;
		if (learningUnitId is not null)
		{
			learningUnit = await conversationRepository.GetLearningUnitContextAsync(learningUnitId, cancellationToken);
			if (learningUnit is null)
				return Result.Failure<PreparedChat>(ConversationErrors.LearningUnitNotFound);
		}

		AiImage? currentImage = null;
		if (attachment is not null)
		{
			currentImage = await LoadImageAsync(attachment.FileKey, attachment.ContentType, cancellationToken);
			if (currentImage is null)
				return Result.Failure<PreparedChat>(ConversationErrors.AttachmentNotFound);
		}

		var vector = await embeddingService.GenerateEmbeddingAsync(content, cancellationToken);

		var saved = await unitOfWork.ExecuteAsync(async () =>
		{
			var conversation = await conversationRepository.GetByLearnerAndVideoAsync(
				learnerId, videoId, cancellationToken);
			if (conversation is null)
			{
				var title = await conversationRepository.GetConversationTitleAsync(
					learnerId, videoId, cancellationToken) ?? content;
				conversation = Conversation.Create(learnerId, videoId, title);
				conversationRepository.Add(conversation);
			}
			else
			{
				conversation.Touch();
			}

			var message = Message.CreateUserMessage(conversation.Id, segmentId, content, learningUnitId);
			conversationRepository.AddMessage(message);

			if (attachment is not null)
			{
				conversationRepository.AddAttachment(Attachment.CreateImage(
					message.Id,
					attachment.FileKey,
					attachment.ContentType.ToLowerInvariant(),
					attachment.FileSize,
					attachment.Timestamp ?? TimeSpan.Zero));
			}

			conversationRepository.AddEmbedding(MessageEmbedding.Create(
				message.Id, content, embeddingService.ModelId, vector));

			return Result.Success((conversation, message));
		}, cancellationToken);

		if (saved.IsFailure)
			return Result.Failure<PreparedChat>(saved.Error);

		var (conv, userMessage) = saved.Value;

		// Latest 10 messages of the same conversation -> multi-turn context.
		var recent = await conversationRepository.GetMessagesAsync(
			conv.Id, null, RecentMessageCount + 1, cancellationToken);

		// Top-5 semantically related messages from the learner's other conversations (other videos).
		var similar = await conversationRepository.GetSimilarMessagesAsync(
			learnerId, conv.Id, vector, userMessage.Id, SimilarMessageCount, cancellationToken);

		var relatedMessages = similar
			.Where(m => !string.IsNullOrWhiteSpace(m.Content))
			.OrderBy(m => m.CreatedAt)
			.Select(m => new AiRelatedMessage(m.Conversation.Video.Title, m.Role == MessageRole.User, m.Content))
			.ToList();

		var history = new List<AiChatTurn>();
		foreach (var m in recent.Where(x => x.Id != userMessage.Id).TakeLast(RecentMessageCount))
		{
			if (string.IsNullOrWhiteSpace(m.Content))
				continue;

			AiImage? image = null;
			if (m.Attachment is not null)
				image = await LoadImageAsync(m.Attachment.FileKey, m.Attachment.ContentType, cancellationToken);

			history.Add(new AiChatTurn(m.Role == MessageRole.User, m.Content, image));
		}

		var video = await conversationRepository.GetVideoContextAsync(videoId, cancellationToken);
		if (video is null)
			return Result.Failure<PreparedChat>(ConversationErrors.SegmentNotFound);

		var aiRequest = new AiChatRequest(
			history,
			new AiChatTurn(true, content, currentImage),
			BuildVideoContext(video),
			BuildTranscriptContext(window, segmentId),
			BuildLearningUnitsContext(learningUnit),
			relatedMessages);

		var validIds = new HashSet<string>(window.Select(s => s.Id.Value.ToString()), StringComparer.OrdinalIgnoreCase);
		if (learningUnit is not null)
			validIds.Add(learningUnit.Id.Value.ToString());

		return Result.Success(new PreparedChat(conv, userMessage, segmentId, aiRequest, validIds));
	}

	// Interactive markers: [[transcript_segment:<id>|text]] / [[learning_unit:<id>|text]].
	private static readonly System.Text.RegularExpressions.Regex MarkerRegex = new(
		@"\[\[(transcript_segment|learning_unit):([0-9a-fA-F\-]{36})\|([^\]\|]*)\]\]",
		System.Text.RegularExpressions.RegexOptions.Compiled);

	// Markers whose id was not part of the context are replaced by their plain display text.
	public static string SanitizeMarkers(PreparedChat prepared, string text)
	{
		return MarkerRegex.Replace(text, m =>
			prepared.ValidReferenceIds.Contains(m.Groups[2].Value) ? m.Value : m.Groups[3].Value);
	}

	// References (distinct by id) found in a sanitized answer, for the frontend.
	public static IReadOnlyList<ReferenceDto> ExtractReferences(PreparedChat prepared, string? content)
	{
		if (string.IsNullOrEmpty(content))
			return [];

		return MarkerRegex.Matches(content)
			.Select(m => new ReferenceDto(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value))
			.Where(r => prepared.ValidReferenceIds.Contains(r.Id))
			.DistinctBy(r => (r.Type, r.Id))
			.ToList();
	}

	// Drops references whose IDs were not part of the context (anti-hallucination).
	public static IReadOnlyList<ReferenceDto> FilterReferences(PreparedChat prepared, IReadOnlyList<AiReference>? references)
	{
		if (references is null)
			return [];

		return references
			.Where(r => (r.Type == "transcript_segment" || r.Type == "learning_unit")
				&& prepared.ValidReferenceIds.Contains(r.Id))
			.Select(r => new ReferenceDto(r.Type, r.Id, r.Reason))
			.ToList();
	}

	public async Task<Result<MessageDto>> CompleteAsync(
		PreparedChat prepared,
		string? text,
		int? inputTokens,
		int? outputTokens,
		bool blocked,
		string? error,
		CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			var reply = Message.CreateAssistantPlaceholder(prepared.Conversation.Id, prepared.SegmentId);
			reply.SetAiRequestPayload(BuildAiRequestPayload(prepared.AiRequest));

			if (error is not null)
				reply.Fail(error);
			else
				reply.Complete(
					blocked || string.IsNullOrWhiteSpace(text) ? BlockedReply : SanitizeMarkers(prepared, text),
					inputTokens,
					outputTokens);

			conversationRepository.AddMessage(reply);
			return Result.Success(ToDto(reply, null));
		}, cancellationToken);
	}

	private static string BuildAiRequestPayload(AiChatRequest request)
	{
		var payload = new
		{
			video_context = request.VideoContext,
			transcript_context = request.TranscriptContext,
			learning_units_context = request.LearningUnitsContext,
			conversation_context = request.History
				.Where(t => !string.IsNullOrWhiteSpace(t.Text))
				.Select(t => new { role = t.IsUser ? "learner" : "assistant", text = t.Text }),
			learner_question = request.Question.Text,
			has_image = request.Question.Image is not null
		};

		return System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions
		{
			Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			WriteIndented = true
		});
	}

	public MessageDto ToDto(Message message, string? attachmentUrl)
	{
		var attachment = message.Attachment is null || attachmentUrl is null
			? null
			: new AttachmentDto(
				attachmentUrl,
				message.Attachment.ContentType,
				message.Attachment.FileSize,
				message.Attachment.Timestamp);

		return new MessageDto(
			message.Id.Value,
			message.TranscriptSegmentId.Value,
			message.LearningUnitId?.Value,
			message.Role.ToString(),
			message.Content,
			message.Status.ToString(),
			message.ErrorMessage,
			message.CreatedAt,
			message.CompletedAt,
			attachment);
	}

	private async Task<AiImage?> LoadImageAsync(string key, string contentType, CancellationToken cancellationToken)
	{
		try
		{
			await using var stream = await storageService.StreamingReadAsync(key, cancellationToken);
			using var ms = new MemoryStream();
			await stream.CopyToAsync(ms, cancellationToken);
			if (ms.Length == 0 || ms.Length > MaxImageBytes)
				return null;
			return new AiImage(ms.ToArray(), contentType);
		}
		catch (Exception) when (!cancellationToken.IsCancellationRequested)
		{
			return null;
		}
	}

	private static string BuildVideoContext(Video video)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Title: {video.Title}");
		if (!string.IsNullOrWhiteSpace(video.Description))
			sb.AppendLine($"Description: {video.Description}");
		sb.AppendLine($"Category: {video.ContentCategory.Name}");
		return sb.ToString();
	}

	private static string BuildLearningUnitsContext(LearningUnit? learningUnit)
	{
		var sb = new StringBuilder();

		if (learningUnit is not null)
		{
			var applied = learningUnit.KnowledgeMeaning;
			var unit = applied.KnowledgeUnit;
			sb.AppendLine($"Learning Unit [id={learningUnit.Id.Value}]");
			sb.AppendLine("The learner is asking about this word/grammar point highlighted in the video.");
			sb.AppendLine($"type: {unit.KnowledgeType}");
			sb.AppendLine($"expression: {unit.Expression}");
			sb.AppendLine($"reading: {unit.Reading}");
			sb.AppendLine("meanings:");
			foreach (var m in unit.Meanings.OrderBy(x => x.SortOrder))
			{
				sb.AppendLine($"- {m.Meaning} / {m.JapaneseMeaning}{(m.Id == applied.Id ? " (applied in this video)" : "")}");
				if (!string.IsNullOrWhiteSpace(m.Description))
					sb.AppendLine($"  {m.Description}");
			}
		}

		return sb.Length == 0 ? "None" : sb.ToString();
	}

	private static string BuildTranscriptContext(IReadOnlyList<TranscriptSegment> window, TranscriptSegmentId targetId)
	{
		var sb = new StringBuilder();

		var speakers = window
			.Select(s => s.TranscriptSpeaker?.Speaker)
			.Where(sp => sp is not null)
			.DistinctBy(sp => sp!.Id)
			.ToList();

		if (speakers.Count > 0)
		{
			sb.AppendLine("Speakers:");
			foreach (var sp in speakers)
			{
				sb.Append($"- {sp!.Name}");
				if (!string.IsNullOrWhiteSpace(sp.Description))
					sb.Append($": {sp.Description}");
				sb.AppendLine();
			}
			sb.AppendLine();
		}

		sb.AppendLine("Transcript (the learner is asking about the segment marked [TARGET]):");
		foreach (var s in window)
		{
			var name = s.TranscriptSpeaker?.Speaker?.Name ?? s.TranscriptSpeaker?.Label ?? "Unknown";
			var target = s.Id == targetId ? "[TARGET] " : "";
			sb.AppendLine($"{target}{name} (id={s.Id.Value}): {s.Text}");
		}

		return sb.ToString();
	}
}
