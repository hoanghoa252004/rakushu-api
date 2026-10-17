using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Material.LearningUnit;

public static class LearningUnitErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "LEARNING_UNIT.NOT_FOUND", "The learning unit was not found.");
}
