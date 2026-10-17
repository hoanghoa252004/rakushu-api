using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;

namespace Rakushu.Domain.Entities.Srs;

public interface ISrsNoteRepository : IBaseRepository<SrsNote, SrsNoteId>
{
    Task<bool> LearningUnitExistsAsync(LearningUnitId learningUnitId, CancellationToken cancellationToken = default);
    Task<(SrsNote? Note, SrsCard? Card)> GetEnrollmentAsync(UserId userId, LearningUnitId learningUnitId, CancellationToken cancellationToken = default);
}
