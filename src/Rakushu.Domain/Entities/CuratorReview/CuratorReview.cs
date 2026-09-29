using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.OovCandidate;
using Rakushu.Domain.Entities.User;
using System;

namespace Rakushu.Domain.Entities.CuratorReview;

public sealed class CuratorReview : Entity<CuratorReviewId>
{
	public OovCandidateId OovCandidateId { get; private set; } = null!;
	public UserId CuratorId { get; private set; } = null!;
	public CuratorDecision Decision { get; private set; }
	public string? EditedTerm { get; private set; }
	public string? EditedReading { get; private set; }
	public string? EditedPos { get; private set; }
	public string? EditedMeaning { get; private set; }
	public string? Comment { get; private set; }
	public DateTimeOffset ReviewedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// Navigation properties
	public User.User? Curator { get; private set; }
	public OovCandidate.OovCandidate? OovCandidate { get; private set; }

	private CuratorReview() { }

	private CuratorReview(
		CuratorReviewId id,
		OovCandidateId oovCandidateId,
		UserId curatorId,
		CuratorDecision decision,
		DateTimeOffset reviewedAt,
		DateTimeOffset createdAt,
		string? editedTerm = null,
		string? editedReading = null,
		string? editedPos = null,
		string? editedMeaning = null,
		string? comment = null) : base(id)
	{
		OovCandidateId = oovCandidateId;
		CuratorId = curatorId;
		Decision = decision;
		ReviewedAt = reviewedAt;
		CreatedAt = createdAt;
		EditedTerm = editedTerm;
		EditedReading = editedReading;
		EditedPos = editedPos;
		EditedMeaning = editedMeaning;
		Comment = comment;
	}

	public static Result<CuratorReview> Create(
		OovCandidateId oovCandidateId,
		UserId curatorId,
		CuratorDecision decision,
		DateTimeOffset reviewedAt,
		string? editedTerm = null,
		string? editedReading = null,
		string? editedPos = null,
		string? editedMeaning = null,
		string? comment = null)
	{
		return Result<CuratorReview>.Success(new CuratorReview(
			CuratorReviewId.Create(),
			oovCandidateId,
			curatorId,
			decision,
			reviewedAt,
			reviewedAt,
			editedTerm,
			editedReading,
			editedPos,
			editedMeaning,
			comment
		));
	}
}
