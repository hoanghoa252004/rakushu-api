namespace Rakushu.Application.Usecases.Learning.Srs.Statistics;

public sealed record CoverageMatrixCellDto(
	string JlptLevel,
	string KnowledgeType,
	int TotalSaved,
	int MasteredCount,
	int MasteryPercentage);

public sealed record SrsCoverageLevelDto(
	string Level,
	int TotalSaved,
	int MasteredCount,
	double MasteryRate);

public sealed record SrsCoverageTypeDto(
	string Type,
	string TypeName,
	int TotalSaved,
	int MasteredCount,
	double MasteryRate);

public sealed record SrsCoverageStatisticsDto(
	int TotalSaved,
	int TotalMastered,
	double OverallMasteryRate,
	IReadOnlyList<CoverageMatrixCellDto> Matrix,
	IReadOnlyList<SrsCoverageLevelDto> ByLevel,
	IReadOnlyList<SrsCoverageTypeDto> ByType);
