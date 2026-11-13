using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;

public sealed class ShadowingAttemptDetail : AggregateRoot<ShadowingAttemptDetailId>
{
	public ShadowingAttemptId ShadowingAttemptId { get; private set; } = null!;
	public ShadowingRoleLineId ShadowingRoleLineId { get; private set; } = null!;
	public string? AudioKey { get; private set; }
	public long? DurationMs { get; private set; }
	public bool IsSkipped { get; private set; } = false;
	public DateTimeOffset? SubmittedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public ShadowingAttempt ShadowingAttempt { get; private set; } = null!;

	public ShadowingRoleLine.ShadowingRoleLine ShadowingRoleLine { get; private set; } = null!;

	private readonly List<ShadowingAssessment.ShadowingAssessmentDetail.ShadowingAssessmentDetail> _assessmentDetails
		= new();

	public IReadOnlyCollection<ShadowingAssessment.ShadowingAssessmentDetail.ShadowingAssessmentDetail>
		AssessmentDetails => _assessmentDetails.AsReadOnly();

	private ShadowingAttemptDetail() { }

	private ShadowingAttemptDetail(
		ShadowingAttemptId attemptId,
		ShadowingRoleLineId roleLineId,
		string? audioKey,
		long? durationMs,
		bool isSkipped,
		DateTimeOffset now) : base(ShadowingAttemptDetailId.Create())
	{
		ShadowingAttemptId = attemptId;
		ShadowingRoleLineId = roleLineId;
		AudioKey = audioKey;
		DurationMs = durationMs;
		IsSkipped = isSkipped;
		SubmittedAt = now;
	}

	public static Domain.Common.Results.Result<ShadowingAttemptDetail> Create(
		ShadowingAttemptId attemptId,
		ShadowingSessionId sessionId,
		Guid memberUserId,
		ShadowingRoleLineId roleLineId,
		string? audioKey,
		long? durationMs,
		bool isSkipped,
		DateTimeOffset now)
	{
		var validation = Validate(audioKey, durationMs, isSkipped);
		if (validation.IsFailure)
			return Domain.Common.Results.Result.Failure<ShadowingAttemptDetail>(validation.Error);

		var detail = isSkipped
			? new ShadowingAttemptDetail(attemptId, roleLineId, null, null, true, now)
			: new ShadowingAttemptDetail(attemptId, roleLineId, audioKey!.Trim(), durationMs, false, now);

		detail.AddDomainEvent(new AttemptDetailCreatedDomainEvent(
			sessionId.Value, attemptId.Value, roleLineId.Value, memberUserId, isSkipped, now));

		return Domain.Common.Results.Result.Success(detail);
	}

	private static Domain.Common.Results.Result Validate(string? audioKey, long? durationMs, bool isSkipped)
	{
		if (isSkipped)
			return Domain.Common.Results.Result.Success();

		if (string.IsNullOrWhiteSpace(audioKey) || audioKey.Trim().Length > 500 || durationMs is null or <= 0)
			return Domain.Common.Results.Result.Failure(ShadowingAttemptErrors.InvalidAudio);
		return Domain.Common.Results.Result.Success();
	}
}
