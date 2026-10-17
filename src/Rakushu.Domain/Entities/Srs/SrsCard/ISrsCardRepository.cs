using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public interface ISrsCardRepository : IBaseRepository<SrsCard, SrsCardId>
{
    Task<IReadOnlyList<SrsCard>> GetDueCardsAsync(UserId userId, DateTimeOffset now, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SrsCard>> GetDueCardsAsync(UserId userId, DateTimeOffset now, int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SrsCard>> GetAllActiveCardsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SrsCard>> GetQuizCardsAsync(UserId userId, DateTimeOffset now, int limit, bool dueOnly, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SrsCard>> GetQuizCardsByIdsAsync(UserId userId, IReadOnlyCollection<Guid> cardIds, DateTimeOffset now, bool dueOnly, CancellationToken cancellationToken = default);
    void AddState(SrsCardState state);
    void AddReviewLog(SrsReviewLog log);
}