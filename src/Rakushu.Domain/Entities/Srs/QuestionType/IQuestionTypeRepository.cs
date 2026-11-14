using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Srs.QuestionType;

public interface IQuestionTypeRepository : IBaseRepository<QuestionType, QuestionTypeId>
{
	Task<IReadOnlyList<QuestionType>> GetActiveQuestionTypesAsync(CancellationToken cancellationToken = default);

	Task<QuestionType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
