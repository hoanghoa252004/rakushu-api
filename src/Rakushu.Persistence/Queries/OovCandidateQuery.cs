using Dapper;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Application.Usecases.Curator.Oov.GetOovCandidates;
using Rakushu.Domain.Entities.OovCandidate;
using Rakushu.Persistence.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Queries;

internal sealed class OovCandidateQuery : IOovCandidateQuery
{
	private readonly IDbConnectionFactory _connection;

	public OovCandidateQuery(IDbConnectionFactory connection)
	{
		_connection = connection;
	}

	public async Task<OovCandidateDetailDto?> GetByIdAsync(OovCandidateId id, CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		const string sql = """
			SELECT 
				o.oov_candidate_id AS Id,
				o.token_id AS TokenId,
				o.term AS Term,
				o.tentative_reading AS TentativeReading,
				o.tentative_pos AS TentativePos,
				o.suggested_meaning AS SuggestedMeaning,
				o.context_snippet AS ContextSnippet,
				o.confidence_score AS ConfidenceScore,
				o.status AS Status,
				o.detected_at AS DetectedAt,
				o.updated_at AS UpdatedAt,
				r.review_id AS ReviewId,
				r.oov_candidate_id AS OovCandidateId,
				r.curator_id AS CuratorId,
				r.decision AS Decision,
				r.edited_term AS EditedTerm,
				r.edited_reading AS EditedReading,
				r.edited_pos AS EditedPos,
				r.edited_meaning AS EditedMeaning,
				r.comment AS Comment,
				r.reviewed_at AS ReviewedAt
			FROM oov_candidates o
			LEFT JOIN curator_reviews r ON o.oov_candidate_id = r.oov_candidate_id
			WHERE o.oov_candidate_id = @Id
			""";

		var result = await connection.QueryAsync<OovCandidateDetailDto, CuratorReviewDto, OovCandidateDetailDto>(
			new CommandDefinition(sql, new { Id = id.Value }, cancellationToken: cancellationToken),
			(cand, review) => new OovCandidateDetailDto(
				cand.Id, cand.TokenId, cand.Term, cand.TentativeReading, cand.TentativePos,
				cand.SuggestedMeaning, cand.ContextSnippet, cand.ConfidenceScore,
				cand.Status, cand.DetectedAt, cand.UpdatedAt,
				review != null && review.ReviewId != Guid.Empty ? review : null
			),
			splitOn: "ReviewId"
		);

		return result.FirstOrDefault();
	}

	public async Task<(IReadOnlyCollection<OovCandidateDto> Items, int TotalCount)> GetOovCandidatesAsync(
		GetOovCandidatesQuery query,
		CancellationToken cancellationToken = default)
	{
		await using var connection = _connection.CreateConnection();

		var sql = """
			SELECT 
				o.oov_candidate_id AS Id,
				o.token_id AS TokenId,
				o.term AS Term,
				o.tentative_reading AS TentativeReading,
				o.tentative_pos AS TentativePos,
				o.suggested_meaning AS SuggestedMeaning,
				o.context_snippet AS ContextSnippet,
				o.confidence_score AS ConfidenceScore,
				o.status AS Status,
				o.detected_at AS DetectedAt,
				o.updated_at AS UpdatedAt
			FROM oov_candidates o
			WHERE 1 = 1
			""";

		var countSql = "SELECT COUNT(*) FROM oov_candidates o WHERE 1 = 1";

		if (!string.IsNullOrWhiteSpace(query.Status))
		{
			sql += " AND o.status = @Status";
			countSql += " AND o.status = @Status";
		}

		if (!string.IsNullOrWhiteSpace(query.SearchTerm))
		{
			sql += " AND (o.term ILIKE @Search OR o.tentative_reading ILIKE @Search OR o.suggested_meaning ILIKE @Search)";
			countSql += " AND (o.term ILIKE @Search OR o.tentative_reading ILIKE @Search OR o.suggested_meaning ILIKE @Search)";
		}

		sql += " ORDER BY o.detected_at DESC OFFSET @Offset LIMIT @Limit";

		var parameters = new
		{
			Status = query.Status,
			Search = $"%{query.SearchTerm}%",
			Offset = (query.PageNumber - 1) * query.PageSize,
			Limit = query.PageSize
		};

		var totalCount = await connection.ExecuteScalarAsync<int>(
			new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

		var items = (await connection.QueryAsync<OovCandidateDto>(
			new CommandDefinition(sql, parameters, cancellationToken: cancellationToken))).ToList();

		return (items, totalCount);
	}
}
