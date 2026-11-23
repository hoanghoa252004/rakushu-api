using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Assisstant.Message;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Assisstant.Attachment;

public sealed partial class Attachment : Entity<AttachmentId>
{
	public MessageId MessageId { get; private set; } = null!;
	public AttachmentType Type { get; private set; }
	public string FileKey { get; private set; } = null!;
	public string ContentType { get; private set; } = null!;
	public long FileSize { get; private set; }
	public TimeSpan Timestamp { get; private set; }
	// NAVIGATION PROPERTIES
	// Message
	public Message.Message Message { get; private set; } = null!;
	private Attachment()
	{
	}

	private Attachment(
		AttachmentId id,
		MessageId messageId,
		string fileKey,
		string contentType,
		long fileSize,
		TimeSpan timestamp)
		: base(id)
	{
		MessageId = messageId;
		Type = AttachmentType.Image;
		FileKey = fileKey;
		ContentType = contentType;
		FileSize = fileSize;
		Timestamp = timestamp;
	}

	public static Attachment CreateImage(
		MessageId messageId,
		string fileKey,
		string contentType,
		long fileSize,
		TimeSpan timestamp)
		=> new(AttachmentId.Create(), messageId, fileKey, contentType, fileSize, timestamp);
}