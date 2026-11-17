using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Conversation;

public sealed partial class Conversation : Entity<ConversationId>
{
	public UserId LearnerId { get; private set; } = null!;
	public VideoId VideoId { get; private set; } = null!;
	public string Title { get; private set; } = null!;
	public DateTime CreatedAt { get; private set; }
	public DateTime UpdatedAt { get; private set; }

	// NAViGATION PROPERTIES
	// Learner
	public User Learner { get; private set; } = null!;

	// Video
	public Video Video { get; private set; } = null!;

	// Messages
	private readonly List<Message.Message> _messages = new List<Message.Message>();
	public IReadOnlyCollection<Message.Message> Messages => _messages.AsReadOnly();

	private Conversation()
	{
	}

	private Conversation(ConversationId id, UserId learnerId, VideoId videoId, string title, DateTime now)
		: base(id)
	{
		LearnerId = learnerId;
		VideoId = videoId;
		Title = title;
		CreatedAt = now;
		UpdatedAt = now;
	}

	public static Conversation Create(UserId learnerId, VideoId videoId, string title)
	{
		var normalized = string.IsNullOrWhiteSpace(title) ? "New conversation" : title.Trim();
		if (normalized.Length > 100)
			normalized = normalized[..100];
		return new Conversation(ConversationId.Create(), learnerId, videoId, normalized, DateTime.UtcNow);
	}

	public void Touch() => UpdatedAt = DateTime.UtcNow;
}