using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

public interface IUniversalPartOfSpeechRepository : IBaseRepository<UniversalPartOfSpeech, UniversalPartOfSpeechId>
{
	Task<UniversalPartOfSpeech?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
