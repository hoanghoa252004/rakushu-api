namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Defines the supported SRS review ratings and validates rating input.
/// </summary>
public static class SrsReviewRating
{
    public static bool IsValid(string? rating) =>
        rating is not null &&
        (rating.Equals("Again", StringComparison.OrdinalIgnoreCase) ||
         rating.Equals("Hard", StringComparison.OrdinalIgnoreCase) ||
         rating.Equals("Good", StringComparison.OrdinalIgnoreCase) ||
         rating.Equals("Easy", StringComparison.OrdinalIgnoreCase));
}
