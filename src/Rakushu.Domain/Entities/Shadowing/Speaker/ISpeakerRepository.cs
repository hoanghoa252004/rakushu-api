using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Shadowing.Speaker;

public interface ISpeakerRepository : IBaseRepository<Speaker, SpeakerId>
{
	Task<bool> CodeExistsAsync(string code, SpeakerId? excludeId, CancellationToken cancellationToken = default);

	Task<bool> IsUsedAsync(SpeakerId id, CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<Speaker> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		string? searchTerm,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<Speaker>> GetByVideoIdAsync(
		Material.Video.VideoId videoId,
		CancellationToken cancellationToken = default);
}
