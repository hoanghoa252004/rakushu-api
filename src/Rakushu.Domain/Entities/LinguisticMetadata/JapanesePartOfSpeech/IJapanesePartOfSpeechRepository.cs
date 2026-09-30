using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

public interface IJapanesePartOfSpeechRepository : IBaseRepository<JapanesePartOfSpeech, JapanesePartOfSpeechId>
{
	Task<JapanesePartOfSpeech?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
