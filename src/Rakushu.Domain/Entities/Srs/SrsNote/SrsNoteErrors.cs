using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs;

/// <summary>
/// Domain errors specific to the SRS note aggregate.
/// </summary>
public static class SrsNoteErrors
{
    public static readonly Error EnrollmentIntegrityConflict = Error.Conflict(
        "SRS.ENROLLMENT_INTEGRITY_CONFLICT",
        "An existing enrollment is incomplete. Resolve its missing SRS card or scheduling state before retrying.");
}
