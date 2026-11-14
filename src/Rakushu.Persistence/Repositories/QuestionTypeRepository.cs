using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Srs.QuestionType;

namespace Rakushu.Persistence.Repositories;

public sealed class QuestionTypeRepository : BaseRepository<QuestionType, QuestionTypeId>, IQuestionTypeRepository
{
	public QuestionTypeRepository(RakushuDbContext context) : base(context) { }

	public async Task<IReadOnlyList<QuestionType>> GetActiveQuestionTypesAsync(CancellationToken cancellationToken = default)
	{
		return await _dbSet
			.AsNoTracking()
			.Where(qt => qt.IsActive)
			.OrderBy(qt => qt.Code)
			.ToListAsync(cancellationToken);
	}

	public async Task<QuestionType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(code))
			return null;

		var normalized = code.Trim().ToUpperInvariant();
		return await _dbSet
			.FirstOrDefaultAsync(qt => qt.Code == normalized, cancellationToken);
	}
}
