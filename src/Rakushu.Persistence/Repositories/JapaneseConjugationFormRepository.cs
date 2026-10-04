using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm;

namespace Rakushu.Persistence.Repositories;

public sealed class JapaneseConjugationFormRepository : BaseRepository<JapaneseConjugationForm, JapaneseConjugationFormId>, IJapaneseConjugationFormRepository
{
	public JapaneseConjugationFormRepository(RakushuDbContext context) : base(context) { }

	public async Task<JapaneseConjugationForm?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.JapaneseConjugationForms
			.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
	}
}
