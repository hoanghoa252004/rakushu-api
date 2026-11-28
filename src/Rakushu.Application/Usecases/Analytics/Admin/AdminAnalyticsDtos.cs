namespace Rakushu.Application.Usecases.Analytics.Admin;

public sealed record AdminMaterialsAnalyticsDto(
	AdminMaterialsKpiDto Kpis,
	AdminMaterialsSourceBreakdownDto SourceBreakdown,
	IReadOnlyList<AdminMaterialsCategoryMetricDto> TopCategories,
	AdminMaterialsStorageBreakdownDto StorageBreakdown,
	IReadOnlyList<AdminMaterialsStatusMetricDto> StatusBreakdown);

public sealed record AdminMaterialsKpiDto(
	int TotalVideos,
	int ActiveVideoCount,
	long TotalDurationSeconds,
	int TotalHours,
	int TotalMinutes,
	int RecentAddedCount,
	int RecentDaysWindow,
	int AvgDurationSeconds,
	string FormattedAvgDuration,
	long TotalStorageBytes,
	string FormattedStorageSize,
	int AudioAssetCount,
	int VideoAndThumbnailCount,
	int AiAvgProcessingSeconds,
	double AiSuccessRate,
	int AiTotalJobs,
	int AiSuccessfulJobs,
	bool HasJobLogs);

public sealed record AdminMaterialsSourceBreakdownDto(
	int CuratedCount,
	double CuratedPercentage,
	int LearnerUploadedCount,
	double LearnerUploadedPercentage,
	int ExternalImportedCount,
	double ExternalImportedPercentage);

public sealed record AdminMaterialsCategoryMetricDto(
	Guid CategoryId,
	string CategoryName,
	string JapaneseName,
	int VideoCount,
	double Percentage);

public sealed record AdminMaterialsStorageBreakdownDto(
	double VideoMb,
	double VideoPercentage,
	int VideoFileCount,
	double AudioMb,
	double AudioPercentage,
	int AudioFileCount,
	double ThumbnailMb,
	double ThumbnailPercentage,
	int ThumbnailFileCount);

public sealed record AdminMaterialsStatusMetricDto(
	string Status,
	int Count,
	double Percentage);
