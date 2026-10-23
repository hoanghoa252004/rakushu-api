using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Persistence.Repositories;

public sealed class SpeakerRepository : BaseRepository<Speaker, SpeakerId>, ISpeakerRepository
{
	public SpeakerRepository(RakushuDbContext context) : base(context)
	{
	}
}
