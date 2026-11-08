using System.Text.Json;

namespace Rakushu.Application.Usecases.Learning.Srs;

public sealed record QuestionMediaMetadataDto(
	double? StartTime = null,
	double? EndTime = null,
	string? SegmentId = null,
	string? TargetWord = null,
	string? Reading = null,
	string? FullSentence = null,
	string? ClozeSentence = null,
	string? VietnameseSub = null);

public static class QuestionMediaMetadataParser
{
	public static QuestionMediaMetadataDto? Parse(string? rawJson)
	{
		if (string.IsNullOrWhiteSpace(rawJson))
			return null;

		try
		{
			using var doc = JsonDocument.Parse(rawJson);
			var root = doc.RootElement;
			if (root.ValueKind != JsonValueKind.Object)
				return null;

			double? startTime = null;
			if (root.TryGetProperty("startTime", out var st) || root.TryGetProperty("start_time", out st))
			{
				if (st.TryGetDouble(out var d)) startTime = d;
			}

			double? endTime = null;
			if (root.TryGetProperty("endTime", out var et) || root.TryGetProperty("end_time", out et))
			{
				if (et.TryGetDouble(out var d)) endTime = d;
			}

			string? segmentId = null;
			if (root.TryGetProperty("segmentId", out var seg) || root.TryGetProperty("segment_id", out seg))
			{
				segmentId = seg.GetString();
			}

			string? targetWord = null;
			if (root.TryGetProperty("targetWord", out var tw) || root.TryGetProperty("target_word", out tw))
			{
				targetWord = tw.GetString();
			}

			string? reading = null;
			if (root.TryGetProperty("reading", out var rd))
			{
				reading = rd.GetString();
			}

			string? fullSentence = null;
			if (root.TryGetProperty("fullSentence", out var fs) || root.TryGetProperty("full_sentence", out fs))
			{
				fullSentence = fs.GetString();
			}

			string? clozeSentence = null;
			if (root.TryGetProperty("clozeSentence", out var cs) || root.TryGetProperty("cloze_sentence", out cs))
			{
				clozeSentence = cs.GetString();
			}

			string? vietnameseSub = null;
			if (root.TryGetProperty("vietnameseSub", out var vs) || root.TryGetProperty("vietnamese_sub", out vs))
			{
				vietnameseSub = vs.GetString();
			}

			return new QuestionMediaMetadataDto(
				startTime,
				endTime,
				segmentId,
				targetWord,
				reading,
				fullSentence,
				clozeSentence,
				vietnameseSub);
		}
		catch
		{
			return null;
		}
	}

	public static QuestionMediaMetadataDto? SanitizeForActiveQuiz(QuestionMediaMetadataDto? metadata)
	{
		if (metadata is null)
			return null;

		return new QuestionMediaMetadataDto(
			StartTime: metadata.StartTime,
			EndTime: metadata.EndTime,
			ClozeSentence: metadata.ClozeSentence);
	}
}
