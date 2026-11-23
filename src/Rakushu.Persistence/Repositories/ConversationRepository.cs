using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using Rakushu.Domain.Entities.Assisstant.Attachment;
using Rakushu.Domain.Entities.Assisstant.Conversation;
using Rakushu.Domain.Entities.Assisstant.Message;
using Rakushu.Domain.Entities.Assisstant.Message.MessageEmbedding;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Repositories;

public sealed class ConversationRepository : BaseRepository<Conversation, ConversationId>, IConversationRepository
{
	public ConversationRepository(RakushuDbContext context) : base(context) { }

	public async Task<Conversation?> GetByLearnerAndVideoAsync(
		UserId learnerId,
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<Conversation>()
			.FirstOrDefaultAsync(c => c.LearnerId == learnerId && c.VideoId == videoId, cancellationToken);
	}

	public async Task<IReadOnlyList<Message>> GetMessagesAsync(
		ConversationId conversationId,
		DateTime? before,
		int limit,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Set<Message>()
			.AsNoTracking()
			.Include(m => m.Attachment)
			.Where(m => m.ConversationId == conversationId);

		if (before.HasValue)
			query = query.Where(m => m.CreatedAt < before.Value);

		var items = await query
			.OrderByDescending(m => m.CreatedAt)
			.Take(limit)
			.ToListAsync(cancellationToken);

		items.Reverse();
		return items;
	}

	public async Task<(IReadOnlyList<Conversation> Items, int Total)> GetConversationsPagedAsync(
		UserId? learnerId,
		VideoId? videoId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Set<Conversation>()
			.AsNoTracking()
			.Include(c => c.Learner)
			.Include(c => c.Video)
			.AsQueryable();

		if (learnerId is not null)
			query = query.Where(c => c.LearnerId == learnerId);
		if (videoId is not null)
			query = query.Where(c => c.VideoId == videoId);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderByDescending(c => c.UpdatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}

	public async Task<Conversation?> GetConversationDetailAsync(
		ConversationId conversationId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<Conversation>()
			.AsNoTracking()
			.Include(c => c.Learner)
			.Include(c => c.Video)
			.FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);
	}

	public async Task<(IReadOnlyList<Message> Items, int Total)> GetMessagesPagedAsync(
		ConversationId conversationId,
		int pageNumber,
		int pageSize,
		string? modelId = null,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Set<Message>()
			.AsNoTracking()
			.Include(m => m.Attachment)
			.Where(m => m.ConversationId == conversationId);

		if (!string.IsNullOrWhiteSpace(modelId))
			query = query.Where(m => m.ModelId == modelId);

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderByDescending(m => m.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}

	public async Task<IReadOnlyDictionary<ConversationId, (long InputTokens, long OutputTokens)>> GetTokenTotalsAsync(
		IReadOnlyCollection<ConversationId> conversationIds,
		CancellationToken cancellationToken = default)
	{
		if (conversationIds.Count == 0)
			return new Dictionary<ConversationId, (long, long)>();

		var ids = conversationIds.ToList();
		var rows = await _context.Set<Message>()
			.AsNoTracking()
			.Where(m => ids.Contains(m.ConversationId))
			.GroupBy(m => m.ConversationId)
			.Select(g => new
			{
				Id = g.Key,
				Input = g.Sum(m => (long?)m.InputTokens) ?? 0,
				Output = g.Sum(m => (long?)m.OutputTokens) ?? 0
			})
			.ToListAsync(cancellationToken);

		return rows.ToDictionary(r => r.Id, r => (r.Input, r.Output));
	}

	public async Task<IReadOnlyList<Message>> GetSimilarMessagesAsync(
		UserId learnerId,
		ConversationId currentConversationId,
		float[] vector,
		MessageId excludeMessageId,
		int top,
		CancellationToken cancellationToken = default)
	{
		var queryVector = new Vector(vector);

		return await _context.Set<Message>()
			.AsNoTracking()
			.Include(m => m.Attachment)
			.Include(m => m.Conversation)
				.ThenInclude(c => c.Video)
			.Where(m => m.Conversation.LearnerId == learnerId
				&& m.ConversationId != currentConversationId
				&& m.Id != excludeMessageId
				&& m.Status == MessageStatus.Completed
				&& m.Embedding != null)
			.OrderBy(m => m.Embedding!.Embedding.CosineDistance(queryVector))
			.Take(top)
			.ToListAsync(cancellationToken);
	}

	public async Task<Video?> GetVideoContextAsync(VideoId videoId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos
			.AsNoTracking()
			.Include(v => v.ContentCategory)
			.Include(v => v.Series)
			.FirstOrDefaultAsync(v => v.Id == videoId, cancellationToken);
	}

	public async Task<IReadOnlyList<TranscriptSegment>> GetSegmentWindowAsync(
		VideoId videoId,
		TranscriptSegmentId segmentId,
		int radius,
		CancellationToken cancellationToken = default)
	{
		var target = await _context.TranscriptSegments
			.AsNoTracking()
			.Where(s => s.Id == segmentId && s.Transcript.VideoId == videoId)
			.Select(s => new { s.TranscriptId, s.Sequence })
			.FirstOrDefaultAsync(cancellationToken);

		if (target is null)
			return [];

		var min = target.Sequence - radius;
		var max = target.Sequence + radius;

		return await _context.TranscriptSegments
			.AsNoTracking()
			.Include(s => s.TranscriptSpeaker!)
				.ThenInclude(ts => ts.Speaker)
			.Where(s => s.TranscriptId == target.TranscriptId && s.Sequence >= min && s.Sequence <= max)
			.OrderBy(s => s.Sequence)
			.ToListAsync(cancellationToken);
	}

	public async Task<Rakushu.Domain.Entities.Material.LearningUnit.LearningUnit?> GetLearningUnitContextAsync(
		Rakushu.Domain.Entities.Material.LearningUnit.LearningUnitId learningUnitId,
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<Rakushu.Domain.Entities.Material.LearningUnit.LearningUnit>()
			.AsNoTrackingWithIdentityResolution()
			.Include(l => l.KnowledgeMeaning)
				.ThenInclude(m => m.KnowledgeUnit)
					.ThenInclude(k => k.Meanings)
			.FirstOrDefaultAsync(l => l.Id == learningUnitId, cancellationToken);
	}

	public async Task<bool> VideoExistsAsync(VideoId videoId, CancellationToken cancellationToken = default)
	{
		return await _context.Videos.AnyAsync(v => v.Id == videoId, cancellationToken);
	}

	public async Task<string?> GetConversationTitleAsync(UserId learnerId, VideoId videoId, CancellationToken cancellationToken = default)
	{
		var name = await _context.Set<User>()
			.Where(u => u.Id == learnerId)
			.Select(u => u.FullName)
			.FirstOrDefaultAsync(cancellationToken);
		var videoTitle = await _context.Videos
			.Where(v => v.Id == videoId)
			.Select(v => v.Title)
			.FirstOrDefaultAsync(cancellationToken);
		return name is null || videoTitle is null ? null : $"{name} - {videoTitle}";
	}

	public void AddMessage(Message message) => _context.Set<Message>().Add(message);

	public void AddAttachment(Attachment attachment) => _context.Set<Attachment>().Add(attachment);

	public void AddEmbedding(MessageEmbedding embedding) => _context.Set<MessageEmbedding>().Add(embedding);
}
