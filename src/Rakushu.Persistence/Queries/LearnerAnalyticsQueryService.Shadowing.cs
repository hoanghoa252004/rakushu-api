using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Common.Time;
using Rakushu.Application.Usecases.Analytics;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Analytics.Shadowing;
using Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Persistence.Queries;

internal sealed partial class LearnerAnalyticsQueryService
{
	private sealed record AssessedAttemptAggregate(
		Guid AttemptId,
		Guid SessionId,
		DateTimeOffset CompletedAt,
		string MaterialName,
		string? MaterialJapaneseName,
		string RoleName,
		int DurationSeconds,
		ShadowingAssessmentType AssessmentType,
		decimal OverallScore,
		decimal AccuracyScore,
		decimal PitchSimilarityScore,
		IReadOnlyList<ShadowingWeakWordDto> WeakWords);

	public async Task<ShadowingAnalyticsDetailResponseDto> GetShadowingAnalyticsAsync(
		UserId userId,
		int? days,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default)
	{
		var raw = await LoadShadowingSummaryRawAsync(userId, days, utcNow, cancellationToken);

		var orderedChrono = raw.AssessedAttempts
			.OrderBy(a => a.CompletedAt)
			.ToList();

		var scoreTrend = orderedChrono
			.Select(a =>
			{
				var localDate = AnalyticsTimeZoneHelper.ToLocalDate(a.CompletedAt, timeZone);
				var combinedTitle = string.IsNullOrWhiteSpace(a.MaterialJapaneseName)
					? a.MaterialName
					: $"{a.MaterialJapaneseName} ({a.MaterialName})";

				return new ShadowingDetailScoreTrendPointDto(
					localDate.ToString("dd/MM", CultureInfo.InvariantCulture),
					a.CompletedAt,
					a.OverallScore,
					a.AccuracyScore,
					a.PitchSimilarityScore,
					combinedTitle,
					a.MaterialJapaneseName,
					a.AttemptId,
					a.SessionId);
			})
			.ToList();

		var history = raw.AssessedAttempts
			.OrderByDescending(a => a.CompletedAt)
			.Select(a => new ShadowingHistoryItemDto(
				a.AttemptId,
				a.SessionId,
				a.MaterialName,
				a.MaterialJapaneseName,
				a.RoleName,
				a.CompletedAt,
				a.DurationSeconds,
				a.AccuracyScore,
				a.PitchSimilarityScore,
				a.OverallScore,
				a.AssessmentType.ToString(),
				a.OverallScore >= 80m ? "PASSED" : "NEEDS_WORK"))
			.ToList();

		int passCount = 0;
		int tier60To79Count = 0;
		int tier30To59Count = 0;
		int under30Count = 0;

		foreach (var attempt in orderedChrono)
		{
			if (attempt.OverallScore >= 80m)
			{
				passCount++;
			}
			else if (attempt.OverallScore >= 60m)
			{
				tier60To79Count++;
			}
			else if (attempt.OverallScore >= 30m)
			{
				tier30To59Count++;
			}
			else
			{
				under30Count++;
			}
		}

		var gradeDistribution = new ScoreGradeDistributionDto(
			passCount,
			tier60To79Count,
			tier30To59Count,
			under30Count);

		int totalLines = raw.CompletedLinesCount + raw.SkippedLinesCount;
		double skipRate = totalLines > 0
			? Math.Round((double)raw.SkippedLinesCount / totalLines * 100.0, 1)
			: 0.0;

		decimal avgOverall = orderedChrono.Count > 0
			? Math.Round(orderedChrono.Average(a => a.OverallScore), 1)
			: 0m;

		return new ShadowingAnalyticsDetailResponseDto(
			raw.TotalAttempts,
			raw.CompletedOrAssessedAttempts,
			raw.TotalRecordedSeconds,
			raw.CompletedLinesCount,
			raw.SkippedLinesCount,
			skipRate,
			avgOverall,
			scoreTrend,
			history,
			gradeDistribution);
	}

	private async Task<LearnerShadowingProgressDto> GetDashboardShadowingProgressAsync(
		UserId userId,
		int days,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken)
	{
		var raw = await LoadShadowingSummaryRawAsync(userId, days, utcNow, cancellationToken);

		var orderedChrono = raw.AssessedAttempts
			.OrderBy(a => a.CompletedAt)
			.ToList();

		int totalLines = raw.CompletedLinesCount + raw.SkippedLinesCount;
		double skipRate = totalLines > 0
			? Math.Round((double)raw.SkippedLinesCount / totalLines * 100.0, 1)
			: 0.0;

		decimal? avgOverall = orderedChrono.Count > 0 ? Math.Round(orderedChrono.Average(a => a.OverallScore), 1) : null;
		decimal? avgAccuracy = orderedChrono.Count > 0 ? Math.Round(orderedChrono.Average(a => a.AccuracyScore), 1) : null;
		decimal? avgPitch = orderedChrono.Count > 0 ? Math.Round(orderedChrono.Average(a => a.PitchSimilarityScore), 1) : null;

		var trendPoints = orderedChrono
			.Select(a =>
			{
				var displayMaterial = string.IsNullOrWhiteSpace(a.MaterialJapaneseName)
					? a.MaterialName
					: $"{a.MaterialJapaneseName} ({a.MaterialName})";

				return new ShadowingScoreTrendPointDto(
					a.AttemptId,
					a.SessionId,
					a.CompletedAt,
					displayMaterial,
					a.AssessmentType.ToString(),
					a.OverallScore,
					a.AccuracyScore,
					a.PitchSimilarityScore,
					a.WeakWords);
			})
			.ToList();

		return new LearnerShadowingProgressDto(
			raw.TotalAttempts,
			orderedChrono.Count,
			raw.CancelledAttempts,
			raw.TotalRecordedSeconds,
			raw.CompletedLinesCount,
			raw.SkippedLinesCount,
			skipRate,
			avgOverall,
			avgAccuracy,
			avgPitch,
			trendPoints);
	}

	private async Task<(
		int TotalAttempts,
		int CompletedOrAssessedAttempts,
		int CancelledAttempts,
		int TotalRecordedSeconds,
		int CompletedLinesCount,
		int SkippedLinesCount,
		IReadOnlyList<AssessedAttemptAggregate> AssessedAttempts)> LoadShadowingSummaryRawAsync(
		UserId userId,
		int? days,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken)
	{
		var attemptsQuery = _context.ShadowingAttempts
			.AsNoTracking()
			.Where(a => a.ShadowingSessionMember.MemberId == userId);

		if (days.HasValue)
		{
			var cutoffUtc = utcNow.AddDays(-days.Value);
			attemptsQuery = attemptsQuery.Where(a => (a.CompletedAt ?? a.StartedAt) >= cutoffUtc);
		}

		var attemptRows = await attemptsQuery
			.AsSplitQuery()
			.Select(a => new
			{
				a.Id,
				a.ShadowingSessionId,
				a.Status,
				a.StartedAt,
				a.CompletedAt,
				MaterialName = a.ShadowingSession.ShadowingMaterial.Name,
				VideoTitle = a.ShadowingSession.ShadowingMaterial.Video.Title,
				SpeakerName = a.ShadowingSessionMember.ShadowingRole != null
					? a.ShadowingSessionMember.ShadowingRole.Speaker.Name
					: null,
				SpeakerJapaneseName = a.ShadowingSessionMember.ShadowingRole != null
					? a.ShadowingSessionMember.ShadowingRole.Speaker.JapaneseName
					: null,
				TotalDurationMs = a.Details
					.Where(d => !d.IsSkipped)
					.Sum(d => (long?)(d.DurationMs ?? 0)) ?? 0L,
				SubmittedLines = a.Details.Count(d => !d.IsSkipped),
				SkippedLines = a.Details.Count(d => d.IsSkipped),
				Assessments = a.Assessments.Select(sa => new
				{
					sa.Id,
					sa.Type,
					sa.OverallScore,
					sa.CreatedAt,
				}).ToList(),
			})
			.ToListAsync(cancellationToken);

		int totalAttempts = attemptRows.Count;
		int completedOrAssessed = attemptRows.Count(a =>
			a.Status == ShadowingAttemptStatus.Completed || a.Status == ShadowingAttemptStatus.Assessed);
		int cancelledAttempts = attemptRows.Count(a => a.Status == ShadowingAttemptStatus.Cancelled);
		int completedLinesCount = attemptRows.Sum(a => a.SubmittedLines);
		int skippedLinesCount = attemptRows.Sum(a => a.SkippedLines);
		int totalRecordedSeconds = (int)Math.Round(attemptRows.Sum(a => a.TotalDurationMs) / 1000.0);

		// Prefer CuratorReevaluation if present, otherwise latest Automated assessment.
		var chosenByAttemptIdx = new Dictionary<int, (ShadowingAssessmentId AssessmentId, ShadowingAssessmentType Type, decimal? OverallScore, DateTimeOffset CreatedAt)>();

		for (int i = 0; i < attemptRows.Count; i++)
		{
			var row = attemptRows[i];
			if (row.Assessments.Count == 0)
				continue;

			var preferred = row.Assessments
				.OrderByDescending(sa => sa.Type == ShadowingAssessmentType.CuratorReevaluation)
				.ThenByDescending(sa => sa.CreatedAt)
				.First();

			chosenByAttemptIdx[i] = (preferred.Id, preferred.Type, preferred.OverallScore, preferred.CreatedAt);
		}

		var assessedResults = new List<AssessedAttemptAggregate>(chosenByAttemptIdx.Count);
		if (chosenByAttemptIdx.Count == 0)
		{
			return (totalAttempts, completedOrAssessed, cancelledAttempts, totalRecordedSeconds, completedLinesCount, skippedLinesCount, assessedResults);
		}

		var selectedIds = chosenByAttemptIdx.Values
			.Select(v => v.AssessmentId)
			.Distinct()
			.ToList();
		var assessmentDetails = await _context.ShadowingAssessmentDetails
			.AsNoTracking()
			.Where(d => selectedIds.Contains(d.ShadowingAssessmentId))
			.Select(d => new
			{
				d.ShadowingAssessmentId,
				d.AccuracyScore,
				d.PitchSimilarityScore,
				d.AccuracyAssessmentResponse,
				d.Feedback,
				LineText = d.ShadowingAttemptDetail.ShadowingRoleLine.Text,
				VietnameseMeaning = d.ShadowingAttemptDetail.ShadowingRoleLine.TranscriptSegments
					.Select(ts => ts.TranscriptSegment.Subtitle != null ? ts.TranscriptSegment.Subtitle.Text : null)
					.FirstOrDefault(),
			})
			.ToListAsync(cancellationToken);

		var detailsByAssessment = assessmentDetails
			.GroupBy(d => d.ShadowingAssessmentId)
			.ToDictionary(g => g.Key, g => g.ToList());

		foreach (var kv in chosenByAttemptIdx)
		{
			var row = attemptRows[kv.Key];
			var chosen = kv.Value;
			var lineDetails = detailsByAssessment.GetValueOrDefault(chosen.AssessmentId, []);

			var accScores = lineDetails.Where(d => d.AccuracyScore.HasValue).Select(d => d.AccuracyScore!.Value).ToList();
			var pitchScores = lineDetails.Where(d => d.PitchSimilarityScore.HasValue).Select(d => d.PitchSimilarityScore!.Value).ToList();

			decimal avgAcc = accScores.Count > 0 ? Math.Round(accScores.Average(), 1) : 0m;
			decimal avgPitch = pitchScores.Count > 0 ? Math.Round(pitchScores.Average(), 1) : 0m;
			decimal overall = chosen.OverallScore.HasValue
				? Math.Round(chosen.OverallScore.Value, 1)
				: lineDetails.Count > 0
					? Math.Round(lineDetails.Average(d => ((d.AccuracyScore ?? 0m) + (d.PitchSimilarityScore ?? 0m)) / 2m), 1)
					: 0m;

			var weakWords = AzureSpeechWeakWordParser.ExtractWeakWords(
				lineDetails.Select(d => new ShadowingAssessmentLineInput(
					d.LineText,
					d.VietnameseMeaning,
					d.AccuracyScore,
					d.AccuracyAssessmentResponse,
					d.PitchSimilarityScore,
					d.Feedback)),
				maxCount: 4);

			var (displayName, jpName) = SplitMaterialTitle(row.MaterialName, row.VideoTitle);
			var roleName = !string.IsNullOrWhiteSpace(row.SpeakerName)
				? row.SpeakerName
				: !string.IsNullOrWhiteSpace(row.SpeakerJapaneseName)
					? row.SpeakerJapaneseName
					: "Nhân vật";

			assessedResults.Add(new AssessedAttemptAggregate(
				row.Id.Value,
				row.ShadowingSessionId.Value,
				row.CompletedAt ?? chosen.CreatedAt,
				displayName,
				jpName,
				roleName,
				(int)Math.Round(row.TotalDurationMs / 1000.0),
				chosen.Type,
				overall,
				avgAcc,
				avgPitch,
				weakWords));
		}

		return (totalAttempts, completedOrAssessed, cancelledAttempts, totalRecordedSeconds, completedLinesCount, skippedLinesCount, assessedResults);
	}

	private static (string DisplayName, string? JapaneseName) SplitMaterialTitle(string materialName, string? videoTitle)
	{
		if (string.IsNullOrWhiteSpace(materialName))
			return (videoTitle ?? "Bài luyện Shadowing", null);

		var openIdx = materialName.IndexOf('(');
		var closeIdx = materialName.LastIndexOf(')');
		if (openIdx > 0 && closeIdx > openIdx + 1)
		{
			var primary = materialName[..openIdx].Trim();
			var secondary = materialName[(openIdx + 1)..closeIdx].Trim();
			if (!string.IsNullOrEmpty(primary) && !string.IsNullOrEmpty(secondary))
			{
				// Assume "JapaneseTitle (VietnameseTitle)" convention.
				return (secondary, primary);
			}
		}

		if (!string.IsNullOrWhiteSpace(videoTitle)
			&& !string.Equals(videoTitle.Trim(), materialName.Trim(), StringComparison.OrdinalIgnoreCase))
		{
			return (materialName.Trim(), videoTitle.Trim());
		}

		return (materialName.Trim(), null);
	}

	public async Task<ShadowingAttemptAssessmentContextModel?> GetAttemptAssessmentsContextAsync(
		ShadowingAttemptId attemptId,
		CancellationToken cancellationToken = default)
	{
		var row = await _context.ShadowingAttempts
			.AsNoTracking()
			.AsSplitQuery()
			.Where(a => a.Id == attemptId)
			.Select(a => new
			{
				AttemptId = a.Id.Value,
				SessionId = a.ShadowingSessionId.Value,
				SessionTitle = a.ShadowingSession.Title,
				MemberId = a.ShadowingSessionMember.MemberId.Value,
				LearnerName = a.ShadowingSessionMember.Member.FullName,
				MaterialName = a.ShadowingSession.ShadowingMaterial.Name,
				VideoTitle = a.ShadowingSession.ShadowingMaterial.Video.Title,
				SpeakerName = a.ShadowingSessionMember.ShadowingRole != null
					? a.ShadowingSessionMember.ShadowingRole.Speaker.Name
					: null,
				SpeakerJapaneseName = a.ShadowingSessionMember.ShadowingRole != null
					? a.ShadowingSessionMember.ShadowingRole.Speaker.JapaneseName
					: null,
				a.Status,
				a.StartedAt,
				a.CompletedAt,
				Details = a.Details.Select(d => new
				{
					AttemptDetailId = d.Id.Value,
					RoleLineId = d.ShadowingRoleLineId.Value,
					d.ShadowingRoleLine.SortOrder,
					LineText = d.ShadowingRoleLine.Text,
					VietnameseMeaning = d.ShadowingRoleLine.TranscriptSegments
						.Select(ts => ts.TranscriptSegment.Subtitle != null ? ts.TranscriptSegment.Subtitle.Text : null)
						.FirstOrDefault(),
					d.ShadowingRoleLine.StartTime,
					d.ShadowingRoleLine.EndTime,
					UserAudioKey = d.AudioKey,
					ReferenceAudioKey = d.ShadowingRoleLine.AudioKey,
					d.DurationMs,
					d.IsSkipped,
					d.SubmittedAt,
				}).ToList(),
				Assessments = a.Assessments.Select(sa => new
				{
					AssessmentId = sa.Id.Value,
					sa.Type,
					sa.OverallScore,
					sa.OveralFeedback,
					sa.CreatedAt,
					AssessmentDetails = sa.Details.Select(sad => new
					{
						AttemptDetailId = sad.ShadowingAttemptDetailId.Value,
						sad.AccuracyScore,
						sad.PitchSimilarityScore,
						sad.AccuracyAssessmentResponse,
						sad.Feedback,
					}).ToList(),
				}).ToList(),
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (row is null)
		{
			return null;
		}

		var (displayMaterialName, materialJapaneseName) = SplitMaterialTitle(row.MaterialName, row.VideoTitle);
		var roleName = !string.IsNullOrWhiteSpace(row.SpeakerName)
			? row.SpeakerName
			: !string.IsNullOrWhiteSpace(row.SpeakerJapaneseName)
				? row.SpeakerJapaneseName
				: "Nhân vật";

		var lines = row.Details
			.Select(d => new ShadowingAttemptLineContextModel(
				d.AttemptDetailId,
				d.RoleLineId,
				d.SortOrder,
				d.LineText,
				d.VietnameseMeaning,
				d.StartTime,
				d.EndTime,
				d.UserAudioKey,
				d.ReferenceAudioKey,
				d.DurationMs,
				d.IsSkipped,
				d.SubmittedAt))
			.ToList();

		var assessments = row.Assessments
			.Select(sa => new ShadowingAssessmentContextModel(
				sa.AssessmentId,
				sa.Type,
				sa.OverallScore,
				sa.OveralFeedback,
				sa.CreatedAt,
				sa.AssessmentDetails
					.Select(sad => new ShadowingAssessmentDetailContextModel(
						sad.AttemptDetailId,
						sad.AccuracyScore,
						sad.PitchSimilarityScore,
						sad.AccuracyAssessmentResponse,
						sad.Feedback))
					.ToList()))
			.ToList();

		return new ShadowingAttemptAssessmentContextModel(
			row.AttemptId,
			row.SessionId,
			row.MemberId,
			displayMaterialName,
			materialJapaneseName,
			row.VideoTitle,
			roleName,
			row.Status,
			row.StartedAt,
			row.CompletedAt,
			lines,
			assessments,
			row.SessionTitle,
			row.LearnerName);
	}
}
