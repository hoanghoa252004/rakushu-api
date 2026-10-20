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

    public static string FromQuality(int quality)
    {
        if (quality is < 0 or > 5)
            throw new ArgumentOutOfRangeException(nameof(quality), "SM-2 quality must be between 0 and 5.");

        return quality switch
        {
            < 3 => "Again",
            3 => "Hard",
            4 => "Good",
            5 => "Easy",
            _ => throw new ArgumentOutOfRangeException(nameof(quality))
        };
    }
}
