using System;

namespace Rakushu.Application.Usecases.Curator.Oov.Common;

public sealed class OovCandidateDto
{
	public Guid Id { get; set; }
	public Guid? TokenId { get; set; }
	public string Term { get; set; } = string.Empty;
	public string? TentativeReading { get; set; }
	public string? TentativePos { get; set; }
	public string? SuggestedMeaning { get; set; }
	public string? ContextSnippet { get; set; }
	public double ConfidenceScore { get; set; }
	public string Status { get; set; } = string.Empty;
	public DateTimeOffset DetectedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }

	public OovCandidateDto() { }

	public OovCandidateDto(
		Guid id,
		Guid? tokenId,
		string term,
		string? tentativeReading,
		string? tentativePos,
		string? suggestedMeaning,
		string? contextSnippet,
		double confidenceScore,
		string status,
		DateTimeOffset detectedAt,
		DateTimeOffset updatedAt)
	{
		Id = id;
		TokenId = tokenId;
		Term = term;
		TentativeReading = tentativeReading;
		TentativePos = tentativePos;
		SuggestedMeaning = suggestedMeaning;
		ContextSnippet = contextSnippet;
		ConfidenceScore = confidenceScore;
		Status = status;
		DetectedAt = detectedAt;
		UpdatedAt = updatedAt;
	}
}

public sealed class CuratorReviewDto
{
	public Guid ReviewId { get; set; }
	public Guid OovCandidateId { get; set; }
	public Guid CuratorId { get; set; }
	public string Decision { get; set; } = string.Empty;
	public string? EditedTerm { get; set; }
	public string? EditedReading { get; set; }
	public string? EditedPos { get; set; }
	public string? EditedMeaning { get; set; }
	public string? Comment { get; set; }
	public DateTimeOffset ReviewedAt { get; set; }

	public CuratorReviewDto() { }

	public CuratorReviewDto(
		Guid reviewId,
		Guid oovCandidateId,
		Guid curatorId,
		string decision,
		string? editedTerm,
		string? editedReading,
		string? editedPos,
		string? editedMeaning,
		string? comment,
		DateTimeOffset reviewedAt)
	{
		ReviewId = reviewId;
		OovCandidateId = oovCandidateId;
		CuratorId = curatorId;
		Decision = decision;
		EditedTerm = editedTerm;
		EditedReading = editedReading;
		EditedPos = editedPos;
		EditedMeaning = editedMeaning;
		Comment = comment;
		ReviewedAt = reviewedAt;
	}
}

public sealed class OovCandidateDetailDto
{
	public Guid Id { get; set; }
	public Guid? TokenId { get; set; }
	public string Term { get; set; } = string.Empty;
	public string? TentativeReading { get; set; }
	public string? TentativePos { get; set; }
	public string? SuggestedMeaning { get; set; }
	public string? ContextSnippet { get; set; }
	public double ConfidenceScore { get; set; }
	public string Status { get; set; } = string.Empty;
	public DateTimeOffset DetectedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
	public CuratorReviewDto? Review { get; set; }

	public OovCandidateDetailDto() { }

	public OovCandidateDetailDto(
		Guid id,
		Guid? tokenId,
		string term,
		string? tentativeReading,
		string? tentativePos,
		string? suggestedMeaning,
		string? contextSnippet,
		double confidenceScore,
		string status,
		DateTimeOffset detectedAt,
		DateTimeOffset updatedAt,
		CuratorReviewDto? review = null)
	{
		Id = id;
		TokenId = tokenId;
		Term = term;
		TentativeReading = tentativeReading;
		TentativePos = tentativePos;
		SuggestedMeaning = suggestedMeaning;
		ContextSnippet = contextSnippet;
		ConfidenceScore = confidenceScore;
		Status = status;
		DetectedAt = detectedAt;
		UpdatedAt = updatedAt;
		Review = review;
	}
}
