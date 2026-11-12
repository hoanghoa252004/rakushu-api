namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsDashboardMetrics;

public sealed record SrsDashboardMetricsDto(
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
