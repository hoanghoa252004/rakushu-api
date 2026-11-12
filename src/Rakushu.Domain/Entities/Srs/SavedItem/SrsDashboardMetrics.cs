namespace Rakushu.Domain.Entities.Srs.SavedItem;

public sealed record SrsDashboardMetrics(
	int TotalDueCount,
	int DueTodayCount,
	int OverdueCount,
	int TotalActiveWords,
	double TrueRetentionRate,
	int SuccessfulReviewsLast30Days,
	int TotalReviewsLast30Days,
	int LeechCount,
	double MasteryRate,
	int MasteredWordsCount);
