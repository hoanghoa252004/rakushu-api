using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranscriptSegmentEntity = Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment;

namespace Rakushu.Domain.Entities.Material.Transcript;

public interface ITranscriptRepository : IBaseRepository<Transcript, TranscriptId>
{
	Task<Transcript?> GetByVideoIdAsync(VideoId videoId, int pageNumber,
	int pageSize, CancellationToken cancellationToken = default);

	Task<TranscriptSegmentEntity?> GetSegmentWithCurationContextByIdAsync(
		TranscriptSegmentId segmentId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<TranscriptSegmentEntity>> GetOverlappingSegmentsWithLearningUnitsAsync(
		VideoId videoId,
		TimeSpan rangeStart,
		TimeSpan rangeEnd,
		CancellationToken cancellationToken = default);
}