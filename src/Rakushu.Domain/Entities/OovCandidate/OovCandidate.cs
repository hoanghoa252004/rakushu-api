using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using System;

namespace Rakushu.Domain.Entities.OovCandidate;

public sealed class OovCandidate : AggregateRoot<OovCandidateId>
{
	public Guid? TokenId { get; private set; }
	public string Term { get; private set; } = null!;
	public string? TentativeReading { get; private set; }
	public string? TentativePos { get; private set; }
	public string? SuggestedMeaning { get; private set; }
	public string? ContextSnippet { get; private set; }
	public double ConfidenceScore { get; private set; }
	public OovStatus Status { get; private set; }
	public DateTimeOffset DetectedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// Navigation property
	public CuratorReview.CuratorReview? CuratorReview { get; private set; }

	private OovCandidate() { }

	private OovCandidate(
		OovCandidateId id,
		string term,
		double confidenceScore,
		OovStatus status,
		DateTimeOffset detectedAt,
		DateTimeOffset updatedAt,
		Guid? tokenId = null,
		string? tentativeReading = null,
		string? tentativePos = null,
		string? suggestedMeaning = null,
		string? contextSnippet = null) : base(id)
	{
		Term = term;
		ConfidenceScore = confidenceScore;
		Status = status;
		DetectedAt = detectedAt;
		UpdatedAt = updatedAt;
		TokenId = tokenId;
		TentativeReading = tentativeReading;
		TentativePos = tentativePos;
		SuggestedMeaning = suggestedMeaning;
		ContextSnippet = contextSnippet;
	}

	public static Result<OovCandidate> Create(
		string term,
		double confidenceScore = 0.0,
		Guid? tokenId = null,
		string? tentativeReading = null,
		string? tentativePos = null,
		string? suggestedMeaning = null,
		string? contextSnippet = null,
		DateTimeOffset? detectedAt = null)
	{
		if (string.IsNullOrWhiteSpace(term))
		{
			return Result.Failure<OovCandidate>(OovCandidateErrors.EmptyTerm);
		}

		var now = detectedAt ?? DateTimeOffset.UtcNow;
		return Result<OovCandidate>.Success(new OovCandidate(
			OovCandidateId.Create(),
			term.Trim(),
			confidenceScore,
			OovStatus.PendingCuratorReview,
			now,
			now,
			tokenId,
			tentativeReading?.Trim(),
			tentativePos?.Trim(),
			suggestedMeaning?.Trim(),
			contextSnippet?.Trim()
		));
	}

	public void Adapt(DateTimeOffset updatedAt)
	{
		Status = OovStatus.Adapted;
		UpdatedAt = updatedAt;
	}

	public void Reject(DateTimeOffset updatedAt)
	{
		Status = OovStatus.Rejected;
		UpdatedAt = updatedAt;
	}

	public void ResolveManual(DateTimeOffset updatedAt)
	{
		Status = OovStatus.ResolvedManual;
		UpdatedAt = updatedAt;
	}

	public void Discard(DateTimeOffset updatedAt)
	{
		Status = OovStatus.Discarded;
		UpdatedAt = updatedAt;
	}

	public void SetReview(CuratorReview.CuratorReview review)
	{
		CuratorReview = review;
	}
}
