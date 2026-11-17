using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Message.MessageEmbedding;

public sealed partial class MessageEmbedding : Entity<MessageEmbeddingId>
{
	public MessageId MessageId { get; private set; } = null!;
	public string SourceContent { get; private set; } = null!;
	public string ModelId { get; private set; } = null!;
	public int Dimensions { get; private set; }
	public Pgvector.Vector Embedding { get; private set; } = null!;

	// NAVIGATION PROPERTIES
	public Message Message { get; private set; } = null!;

	private MessageEmbedding()
	{
	}

	private MessageEmbedding(
		MessageEmbeddingId id,
		MessageId messageId,
		string sourceContent,
		string modelId,
		float[] embedding)
		: base(id)
	{
		MessageId = messageId;
		SourceContent = sourceContent;
		ModelId = modelId;
		Dimensions = embedding.Length;
		Embedding = new Pgvector.Vector(embedding);
	}

	public static MessageEmbedding Create(
		MessageId messageId,
		string sourceContent,
		string modelId,
		float[] embedding)
		=> new(MessageEmbeddingId.Create(), messageId, sourceContent, modelId, embedding);
}