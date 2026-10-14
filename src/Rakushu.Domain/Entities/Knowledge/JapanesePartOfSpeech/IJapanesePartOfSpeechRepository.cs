using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;

public interface IJapanesePartOfSpeechRepository : IBaseRepository<JapanesePartOfSpeech, JapanesePartOfSpeechId>
{
	Task<JapanesePartOfSpeech?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
