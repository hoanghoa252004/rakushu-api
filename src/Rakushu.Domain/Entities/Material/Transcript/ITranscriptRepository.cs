using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Transcript;

public interface ITranscriptRepository : IBaseRepository<Transcript, TranscriptId>
{
	Task<Transcript?> GetByVideoIdAsync(VideoId videoId, CancellationToken cancellationToken = default);
}