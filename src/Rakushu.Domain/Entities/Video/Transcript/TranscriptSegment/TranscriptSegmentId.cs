using Rakushu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

public sealed class TranscriptSegmentId : StronglyTypedId<Guid>
{
	private TranscriptSegmentId(Guid value) : base(value)
	{
	}

	public static TranscriptSegmentId Create() => new(Guid.NewGuid());

	public static TranscriptSegmentId From(Guid value) => new(value);
}