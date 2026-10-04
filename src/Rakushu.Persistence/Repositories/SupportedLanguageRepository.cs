using Rakushu.Domain.Entities.SupportedLanguage;

namespace Rakushu.Persistence.Repositories;

public sealed class SupportedLanguageRepository : BaseRepository<SupportedLanguage, SupportedLanguageId>, ISupportedLanguageRepository
{
	public SupportedLanguageRepository(RakushuDbContext context) : base(context) { }
}
