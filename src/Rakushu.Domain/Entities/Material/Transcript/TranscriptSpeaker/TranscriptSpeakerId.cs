using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;

public sealed class TranscriptSpeakerId : StronglyTypedId<Guid>
{
	private TranscriptSpeakerId(Guid value) : base(value)
	{
	}

	public static TranscriptSpeakerId Create() => new(Guid.NewGuid());

	public static TranscriptSpeakerId From(Guid value) => new(value);
}