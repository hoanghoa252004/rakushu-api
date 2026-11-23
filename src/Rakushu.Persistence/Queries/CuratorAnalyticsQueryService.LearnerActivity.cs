using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Common.Time;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Persistence.Queries;

internal sealed partial class CuratorAnalyticsQueryService
{
	private static readonly string[] CanonicalJlptLevels = ["N5", "N4", "N3", "N2", "N1"];

	private sealed record AnsweredQuizItemProjection(
		Guid BankQuestionId,
		string QuestionTypeCode,
		string QuestionTypeName,
		string PromptText,
		string? KnowledgeUnitExpression,
		string? KnowledgeUnitReading,
		string JlptLevel,
		Guid UserId,
		bool IsCorrect);

	private async Task<List<AnsweredQuizItemProjection>> LoadAnsweredQuizItemsInWindowAsync(
		DateTimeOffset windowStartUtc,
		CancellationToken cancellationToken)
	{
		return await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(qai => qai.QuizAttempt.Status != QuizAttemptStatus.Canceled
				&& qai.IsCorrect != null
				&& (qai.AnsweredAt ?? qai.QuizAttempt.CreatedAt) >= windowStartUtc)
			.Select(qai => new AnsweredQuizItemProjection(
				qai.Question.BankQuestionId.Value,
				qai.Question.BankQuestion.QuestionType.Code,
				qai.Question.BankQuestion.QuestionType.Name,
				qai.Question.BankQuestion.Question,
				qai.Question.SavedItem.KnowledgeMeaning.KnowledgeUnit.Expression,
				qai.Question.SavedItem.KnowledgeMeaning.KnowledgeUnit.Reading,
				qai.Question.SavedItem.KnowledgeMeaning.KnowledgeUnit.ProficiencyLevel != null
					? qai.Question.SavedItem.KnowledgeMeaning.KnowledgeUnit.ProficiencyLevel.Code
					: "N3",
				qai.QuizAttempt.UserId.Value,
				qai.IsCorrect!.Value))
			.ToListAsync(cancellationToken);
	}

	private static List<CuratorHighErrorQuestionItemDto> AggregateHighErrorQuestions(
		IReadOnlyList<AnsweredQuizItemProjection> answeredItems,
		int minAnswers)
	{
		return answeredItems
			.GroupBy(x => x.BankQuestionId)
			.Select(g =>
			{
				var first = g.First();
				int total = g.Count();
				int wrong = g.Count(x => !x.IsCorrect);
				int distinctLearnersWrong = g
					.Where(x => !x.IsCorrect)
					.Select(x => x.UserId)
					.Distinct()
					.Count();
				double errorRate = total > 0 ? Math.Round((double)wrong / total * 100.0, 1) : 0.0;

				return new CuratorHighErrorQuestionItemDto(
					g.Key,
					first.QuestionTypeCode,
					first.QuestionTypeName,
					TruncateText(first.PromptText, 180),
					first.KnowledgeUnitExpression,
					first.KnowledgeUnitReading,
					(first.JlptLevel ?? "N3").Trim().ToUpperInvariant(),
					total,
					wrong,
					errorRate,
					distinctLearnersWrong);
			})
			.Where(x => x.WrongAnswers > 0 && x.TotalAnswers >= minAnswers)
			.ToList();
	}

	private async Task<CuratorLearnerActivityAnalyticsDto> GetLearnerActivitySectionAsync(
		DateTimeOffset windowStartUtc,
		CancellationToken cancellationToken)
	{
		// 1. Finished QuizAttempts in period across all learners.
		var completedAttempts = await _context.QuizAttempts
			.AsNoTracking()
			.Where(qa => (qa.Status == QuizAttemptStatus.Completed || qa.Status == QuizAttemptStatus.Expired)
				&& (qa.CompletedAt ?? qa.UpdatedAt) >= windowStartUtc)
			.Select(qa => new { qa.UserId })
			.ToListAsync(cancellationToken);

		int completedQuizAttempts = completedAttempts.Count;
		int activeQuizLearnersCount = completedAttempts
			.Select(x => x.UserId.Value)
			.Distinct()
			.Count();

		// 2. Per-item QuizAttemptItems in period with BankQuestion & JLPT Level.
		var answeredItems = await LoadAnsweredQuizItemsInWindowAsync(windowStartUtc, cancellationToken);

		int totalAnsweredItems = answeredItems.Count;
		int totalCorrectItems = answeredItems.Count(x => x.IsCorrect);
		int totalWrongItems = totalAnsweredItems - totalCorrectItems;
		double avgQuizAccuracyPercent = totalAnsweredItems > 0
			? Math.Round((double)totalCorrectItems / totalAnsweredItems * 100.0, 1)
			: 0.0;

		if (activeQuizLearnersCount == 0 && totalAnsweredItems > 0)
		{
			activeQuizLearnersCount = answeredItems.Select(x => x.UserId).Distinct().Count();
		}

		// 3. Breakdown by JLPT Level (N5 -> N1).
		var itemsByLevel = answeredItems
			.GroupBy(x => (x.JlptLevel ?? "N3").Trim().ToUpperInvariant(), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

		var levelBreakdown = CanonicalJlptLevels
			.Select(levelCode =>
			{
				if (!itemsByLevel.TryGetValue(levelCode, out var list) || list.Count == 0)
				{
					return new CuratorLevelPerformanceDto(levelCode, 0, 0, 0.0, 0);
				}

				int correct = list.Count(x => x.IsCorrect);
				double acc = Math.Round((double)correct / list.Count * 100.0, 1);
				int learners = list.Select(x => x.UserId).Distinct().Count();
				return new CuratorLevelPerformanceDto(levelCode, list.Count, correct, acc, learners);
			})
			.ToList();

		// 4. All high-error BankQuestions for exact total & per-level counts + guaranteed Top 4 per level preview.
		var allHighError = AggregateHighErrorQuestions(answeredItems, minAnswers: 1)
			.OrderByDescending(x => x.ErrorRatePercent)
			.ThenByDescending(x => x.WrongAnswers)
			.ThenByDescending(x => x.TotalAnswers)
			.ToList();

		var levelErrorCounts = CanonicalJlptLevels
			.Select(code => new CuratorLevelErrorCountDto(
				code,
				allHighError.Count(x => string.Equals(x.JlptLevel, code, StringComparison.OrdinalIgnoreCase))))
			.ToList();

		var previewHighError = allHighError
			.Take(4)
			.Concat(CanonicalJlptLevels.SelectMany(lvl => allHighError
				.Where(x => string.Equals(x.JlptLevel, lvl, StringComparison.OrdinalIgnoreCase))
				.Take(4)))
			.DistinctBy(x => x.BankQuestionId)
			.OrderByDescending(x => x.ErrorRatePercent)
			.ThenByDescending(x => x.WrongAnswers)
			.ThenByDescending(x => x.TotalAnswers)
			.ToList();

		return new CuratorLearnerActivityAnalyticsDto(
			completedQuizAttempts,
			totalAnsweredItems,
			totalWrongItems,
			avgQuizAccuracyPercent,
			activeQuizLearnersCount,
			allHighError.Count,
			levelBreakdown,
			levelErrorCounts,
			previewHighError);
	}

	public async Task<CuratorHighErrorQuestionsResponseDto> GetHighErrorQuestionsAsync(
		int days,
		TimeZoneInfo timeZone,
		DateTimeOffset utcNow,
		string? level,
		string? questionTypeCode,
		string? search,
		int minAnswers,
		string? sortBy,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var localToday = AnalyticsTimeZoneHelper.GetLocalToday(utcNow, timeZone);
		var startLocalDate = localToday.AddDays(-(days - 1));
		var (windowStartUtc, _) = AnalyticsTimeZoneHelper.GetUtcBoundsForLocalDate(startLocalDate, timeZone);

		var answeredItems = await LoadAnsweredQuizItemsInWindowAsync(windowStartUtc, cancellationToken);
		var baseHighError = AggregateHighErrorQuestions(answeredItems, Math.Max(1, minAnswers));

		// Apply questionTypeCode & search BEFORE levelCounts so level pills reflect current search/type filters.
		IEnumerable<CuratorHighErrorQuestionItemDto> preLevelFiltered = baseHighError;

		if (!string.IsNullOrWhiteSpace(questionTypeCode) && !string.Equals(questionTypeCode, "ALL", StringComparison.OrdinalIgnoreCase))
		{
			string normalizedType = questionTypeCode.Trim();
			preLevelFiltered = preLevelFiltered.Where(x =>
				string.Equals(x.QuestionTypeCode, normalizedType, StringComparison.OrdinalIgnoreCase));
		}

		if (!string.IsNullOrWhiteSpace(search))
		{
			string term = search.Trim();
			preLevelFiltered = preLevelFiltered.Where(x =>
				x.PromptText.Contains(term, StringComparison.OrdinalIgnoreCase)
				|| (x.KnowledgeUnitExpression != null && x.KnowledgeUnitExpression.Contains(term, StringComparison.OrdinalIgnoreCase))
				|| (x.KnowledgeUnitReading != null && x.KnowledgeUnitReading.Contains(term, StringComparison.OrdinalIgnoreCase)));
		}

		var preLevelList = preLevelFiltered.ToList();

		var levelCounts = CanonicalJlptLevels
			.Select(code => new CuratorLevelErrorCountDto(
				code,
				preLevelList.Count(x => string.Equals(x.JlptLevel, code, StringComparison.OrdinalIgnoreCase))))
			.ToList();

		IEnumerable<CuratorHighErrorQuestionItemDto> filtered = preLevelList;
		if (!string.IsNullOrWhiteSpace(level) && !string.Equals(level, "ALL", StringComparison.OrdinalIgnoreCase))
		{
			string normalizedLevel = level.Trim().ToUpperInvariant();
			filtered = filtered.Where(x => string.Equals(x.JlptLevel, normalizedLevel, StringComparison.OrdinalIgnoreCase));
		}

		string normalizedSort = (sortBy ?? "errorRate").Trim().ToLowerInvariant();
		IOrderedEnumerable<CuratorHighErrorQuestionItemDto> ordered = normalizedSort switch
		{
			"wronganswers" => filtered
				.OrderByDescending(x => x.WrongAnswers)
				.ThenByDescending(x => x.ErrorRatePercent)
				.ThenByDescending(x => x.TotalAnswers),
			"totalanswers" => filtered
				.OrderByDescending(x => x.TotalAnswers)
				.ThenByDescending(x => x.ErrorRatePercent)
				.ThenByDescending(x => x.WrongAnswers),
			_ => filtered
				.OrderByDescending(x => x.ErrorRatePercent)
				.ThenByDescending(x => x.WrongAnswers)
				.ThenByDescending(x => x.TotalAnswers),
		};

		var finalList = ordered.ToList();
		int totalCount = finalList.Count;
		int totalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 1;
		int safePage = Math.Clamp(page, 1, totalPages);

		var pagedItems = finalList
			.Skip((safePage - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		return new CuratorHighErrorQuestionsResponseDto(
			days,
			totalCount,
			safePage,
			pageSize,
			totalPages,
			levelCounts,
			pagedItems);
	}
}
