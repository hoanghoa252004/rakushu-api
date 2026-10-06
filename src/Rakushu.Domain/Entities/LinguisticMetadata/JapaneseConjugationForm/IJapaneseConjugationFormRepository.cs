using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

public interface IJapaneseConjugationFormRepository : IBaseRepository<JapaneseConjugationForm, JapaneseConjugationFormId>
{
	Task<JapaneseConjugationForm?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
