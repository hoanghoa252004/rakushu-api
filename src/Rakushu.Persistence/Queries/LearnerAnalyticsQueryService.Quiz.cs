using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Common.Time;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Persistence.Queries;

internal sealed partial class LearnerAnalyticsQueryService
{
	private const int MaxQuestionTimeSpentMs = 300_000;

	private static readonly string[] CanonicalQuestionTypeOrder =
	[
		"WORD_TO_MEANING",
		"KANJI_TO_READING",
		"READING_TO_KANJI",
		"TYPE_READING",
		"TEXT_CLOZE",
		"VIDEO_CLOZE",
		"AUDIO_DICTATION",
	];

	private sealed record QuizAttemptItemProjection(
		string QuestionTypeCode,
		Guid SavedItemId,
		string Expression,
		string Reading,
		string Meaning,
		bool IsCorrect,
		int TimeSpentMs);

	private sealed record QuestionTypeMasterProjection(
		string Code,
		string Name,
		int ExpectedDurationSeconds);

	public async Task<QuestionTypeDetailResponseDto> GetQuizDiagnosticsAsync(
		UserId userId,
		int? days,
		string? questionTypeCode,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken = default)
	{
		DateTimeOffset? cutoffUtc = days.HasValue ? utcNow.AddDays(-days.Value) : null;
		var (questionTypes, items) = await LoadQuizDiagnosticRawDataAsync(userId, cutoffUtc, cancellationToken);

		var summaries = BuildQuestionTypeSummaries(questionTypes, items);
		var wordsByType = BuildWordsByTypeDictionary(questionTypes, items);

		var weakest = summaries.FirstOrDefault(s => s.IsWeakest) ?? summaries.FirstOrDefault();
		string? defaultSelectedWeakestTypeId = weakest?.Code;

		IReadOnlyList<QuestionTypeWordEvidenceDto> selectedWords = [];
		if (!string.IsNullOrWhiteSpace(questionTypeCode))
		{
			var normalizedCode = questionTypeCode.Trim().ToUpperInvariant();
			selectedWords = wordsByType.GetValueOrDefault(normalizedCode, []);
		}
		else if (weakest is not null)
		{
			selectedWords = wordsByType.GetValueOrDefault(weakest.Code, []);
		}

		return new QuestionTypeDetailResponseDto(
			summaries,
			wordsByType,
			selectedWords,
			defaultSelectedWeakestTypeId);
	}

	private async Task<LearnerPracticePerformanceDto> GetPracticePerformanceSectionAsync(
		UserId userId,
		int days,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		CancellationToken cancellationToken)
	{
		var cutoffUtc = utcNow.AddDays(-days);

		// 1. Finished QuizAttempts in period for summary & daily trend.
		var attempts = await _context.QuizAttempts
			.AsNoTracking()
			.Where(qa => qa.UserId == userId
				&& (qa.Status == QuizAttemptStatus.Completed || qa.Status == QuizAttemptStatus.Expired)
				&& (qa.CompletedAt ?? qa.UpdatedAt) >= cutoffUtc)
			.Select(qa => new
			{
				qa.Mode,
				qa.Score,
				qa.TotalItems,
				qa.AnsweredItems,
				qa.CorrectItems,
				EffectiveTimestamp = qa.CompletedAt ?? qa.UpdatedAt,
			})
			.ToListAsync(cancellationToken);

		// 2. Per-item QuizAttemptItem records across active QuestionTypes.
		var (questionTypes, answerItems) = await LoadQuizDiagnosticRawDataAsync(userId, cutoffUtc, cancellationToken);
		var summaries = BuildQuestionTypeSummaries(questionTypes, answerItems);
		var wordsByType = BuildWordsByTypeDictionary(questionTypes, answerItems);

		double ComputeAttemptScore(decimal? storedScore, int totalItems, int correctItems)
		{
			if (storedScore.HasValue)
				return (double)storedScore.Value;
			return totalItems > 0 ? Math.Round((double)correctItems / totalItems * 100.0, 1) : 0.0;
		}

		var attemptWithScores = attempts.Select(a => new
		{
			a.Mode,
			a.EffectiveTimestamp,
			Score = ComputeAttemptScore(a.Score, a.TotalItems, a.CorrectItems),
		}).ToList();

		double overallAvgScore = attemptWithScores.Count > 0
			? Math.Round(attemptWithScores.Average(x => x.Score), 1)
			: 0.0;

		var byMode = new[] { QuizAttemptMode.SrsReview, QuizAttemptMode.PracticeAll }
			.Select(mode =>
			{
				var matching = attemptWithScores.Where(x => x.Mode == mode).ToList();
				return new QuizModeSummaryDto(
					mode.ToString(),
					matching.Count,
					matching.Count > 0 ? Math.Round(matching.Average(x => x.Score), 1) : 0.0);
			})
			.ToList();

		var quizSummary = new QuizSummaryDto(
			attemptWithScores.Count,
			answerItems.Count,
			overallAvgScore,
			byMode);

		var scoreTrend = attemptWithScores
			.GroupBy(x => (
				Date: AnalyticsTimeZoneHelper.ToLocalDate(x.EffectiveTimestamp, timeZone),
				x.Mode))
			.OrderBy(g => g.Key.Date)
			.ThenBy(g => g.Key.Mode)
			.Select(g => new QuizScoreTrendPointDto(
				g.Key.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
				g.Key.Mode.ToString(),
				g.Count(),
				Math.Round(g.Average(x => x.Score), 1)))
			.ToList();

		var byQuestionType = summaries
			.Select(s =>
			{
				var wrongWords = wordsByType
					.GetValueOrDefault(s.Code, [])
					.Where(w => w.WrongCount > 0)
					.Take(5)
					.Select(w => new QuizWeakWordDto(
						w.SavedItemId,
						w.Expression,
						w.Reading,
						w.Meaning,
						w.WrongCount,
						w.TotalTimesTested,
						w.AvgTimeSeconds,
						$"Sai {w.WrongCount} lần"))
					.ToList();

				return new QuestionTypeDiagnosticDto(
					s.Code,
					s.Name,
					s.TotalAnswered,
					s.CorrectCount,
					s.AccuracyRate,
					s.AvgTimeSpentSeconds,
					s.ExpectedDurationSeconds,
					wrongWords,
					s.PerformanceTier,
					s.IsOverTime);
			})
			.ToList();

		// 3. Flashcard Summary in window (sequential DbContext access).
		var completedFlashcardSessions = await _context.FlashcardSessions
			.AsNoTracking()
			.CountAsync(fs => fs.UserId == userId
				&& fs.Status == FlashcardSessionStatus.Completed
				&& (fs.CompletedAt ?? fs.CreatedAt) >= cutoffUtc, cancellationToken);

		var flashcardRatings = await _context.FlashcardSessionItems
			.AsNoTracking()
			.Where(fsi => fsi.FlashcardSession.UserId == userId
				&& fsi.FlashcardSession.Status != FlashcardSessionStatus.Canceled
				&& fsi.ReviewedAt >= cutoffUtc)
			.GroupBy(fsi => fsi.Rating)
			.Select(g => new { Rating = g.Key, Count = g.Count() })
			.ToListAsync(cancellationToken);

		var ratingMap = flashcardRatings.ToDictionary(x => x.Rating, x => x.Count);
		int forgot = ratingMap.GetValueOrDefault(FlashcardRating.Forgot, 0);
		int hard = ratingMap.GetValueOrDefault(FlashcardRating.Hard, 0);
		int easy = ratingMap.GetValueOrDefault(FlashcardRating.Easy, 0);

		var flashcardSummary = new FlashcardSummaryDto(
			completedFlashcardSessions,
			forgot + hard + easy,
			new FlashcardRatingDistributionDto(forgot, hard, easy));

		return new LearnerPracticePerformanceDto(
			quizSummary,
			scoreTrend,
			byQuestionType,
			flashcardSummary);
	}

	private async Task<(IReadOnlyList<QuestionTypeMasterProjection> Types, IReadOnlyList<QuizAttemptItemProjection> Items)>
		LoadQuizDiagnosticRawDataAsync(
			UserId userId,
			DateTimeOffset? cutoffUtc,
			CancellationToken cancellationToken)
	{
		var rawTypes = await _context.QuestionTypes
			.AsNoTracking()
			.Where(qt => qt.IsActive)
			.Select(qt => new QuestionTypeMasterProjection(
				qt.Code,
				qt.Name,
				qt.ExpectedDurationSeconds))
			.ToListAsync(cancellationToken);

		var orderedTypes = rawTypes
			.OrderBy(t =>
			{
				int idx = Array.IndexOf(CanonicalQuestionTypeOrder, t.Code.ToUpperInvariant());
				return idx >= 0 ? idx : CanonicalQuestionTypeOrder.Length;
			})
			.ThenBy(t => t.Name, StringComparer.Ordinal)
			.ToList();

		var itemQuery = _context.QuizAttemptItems
			.AsNoTracking()
			.Where(qai => qai.QuizAttempt.UserId == userId
				&& qai.QuizAttempt.Status != QuizAttemptStatus.Canceled
				&& qai.IsCorrect != null);

		if (cutoffUtc.HasValue)
		{
			itemQuery = itemQuery.Where(qai => (qai.AnsweredAt ?? qai.QuizAttempt.CreatedAt) >= cutoffUtc.Value);
		}

		var rawItems = await itemQuery
			.Select(qai => new QuizAttemptItemProjection(
				qai.Question.BankQuestion.QuestionType.Code,
				qai.Question.SavedItemId.Value,
				qai.Question.SavedItem.KnowledgeMeaning.KnowledgeUnit.Expression,
				qai.Question.SavedItem.KnowledgeMeaning.KnowledgeUnit.Reading,
				qai.Question.SavedItem.KnowledgeMeaning.Meaning,
				qai.IsCorrect!.Value,
				qai.TimeSpentMs))
			.ToListAsync(cancellationToken);

		return (orderedTypes, rawItems);
	}

	private static List<QuestionTypeSummaryDto> BuildQuestionTypeSummaries(
		IReadOnlyList<QuestionTypeMasterProjection> questionTypes,
		IReadOnlyList<QuizAttemptItemProjection> items)
	{
		var itemsByType = items
			.GroupBy(i => i.QuestionTypeCode.ToUpperInvariant())
			.ToDictionary(g => g.Key, g => g.ToList());

		var provisional = questionTypes.Select(qt =>
		{
			var list = itemsByType.GetValueOrDefault(qt.Code.ToUpperInvariant(), []);
			int total = list.Count;
			int correct = list.Count(x => x.IsCorrect);
			double accuracy = total > 0 ? Math.Round((double)correct / total * 100.0, 1) : 0.0;
			var timed = list.Where(x => x.TimeSpentMs > 0).Select(x => Math.Min(x.TimeSpentMs, MaxQuestionTimeSpentMs)).ToList();
			double avgSec = timed.Count > 0 ? Math.Round(timed.Average() / 1000.0, 1) : 0.0;

			return (qt.Code, qt.Name, qt.ExpectedDurationSeconds, Total: total, Correct: correct, Accuracy: accuracy, AvgSec: avgSec);
		}).ToList();

		string? weakestCode = provisional
			.Where(x => x.Total > 0)
			.OrderBy(x => x.Accuracy)
			.ThenByDescending(x => x.Total)
			.ThenByDescending(x => x.AvgSec)
			.Select(x => x.Code)
			.FirstOrDefault();

		return provisional
			.Select(x => new QuestionTypeSummaryDto(
				x.Code,
				x.Name,
				x.ExpectedDurationSeconds,
				x.Total,
				x.Correct,
				x.Accuracy,
				x.AvgSec,
				weakestCode is not null && string.Equals(x.Code, weakestCode, StringComparison.OrdinalIgnoreCase),
				ResolvePerformanceTier(x.Accuracy),
				x.AvgSec > x.ExpectedDurationSeconds))
			.ToList();
	}

	private static string ResolvePerformanceTier(double accuracyRate)
	{
		if (accuracyRate >= 90.0)
		{
			return "EXCELLENT";
		}

		if (accuracyRate >= 80.0)
		{
			return "GOOD";
		}

		if (accuracyRate >= 70.0)
		{
			return "AVERAGE";
		}

		return "WEAK";
	}

	private static IReadOnlyDictionary<string, IReadOnlyList<QuestionTypeWordEvidenceDto>> BuildWordsByTypeDictionary(
		IReadOnlyList<QuestionTypeMasterProjection> questionTypes,
		IReadOnlyList<QuizAttemptItemProjection> items)
	{
		var result = new Dictionary<string, IReadOnlyList<QuestionTypeWordEvidenceDto>>(StringComparer.OrdinalIgnoreCase);

		foreach (var qt in questionTypes)
		{
			var code = qt.Code.ToUpperInvariant();
			var words = items
				.Where(i => string.Equals(i.QuestionTypeCode, qt.Code, StringComparison.OrdinalIgnoreCase))
				.GroupBy(i => i.SavedItemId)
				.Select(g =>
				{
					var first = g.First();
					int total = g.Count();
					int correct = g.Count(x => x.IsCorrect);
					int wrong = total - correct;
					var timed = g.Where(x => x.TimeSpentMs > 0).Select(x => Math.Min(x.TimeSpentMs, MaxQuestionTimeSpentMs)).ToList();
					double avgSec = timed.Count > 0 ? Math.Round(timed.Average() / 1000.0, 1) : 0.0;

					return new QuestionTypeWordEvidenceDto(
						g.Key,
						first.Expression,
						first.Reading,
						first.Meaning,
						total,
						correct,
						wrong,
						avgSec);
				})
				.OrderByDescending(w => w.WrongCount)
				.ThenByDescending(w => w.AvgTimeSeconds)
				.ThenBy(w => w.Expression, StringComparer.Ordinal)
				.ToList();

			result[code] = words;
		}

		return result;
	}
}
