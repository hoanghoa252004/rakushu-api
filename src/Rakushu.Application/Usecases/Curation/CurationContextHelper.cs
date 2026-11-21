using System.Text.Json;
using System.Text.Json.Serialization;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation;

public static class CurationContextHelper
{
	public const string OovCategory = "Oov";

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public static string SerializeBankQuestion(BankQuestion question)
	{
		var snapshot = new CurationTargetSnapshotDto(
			TargetType: nameof(TargetType.BankQuestion),
			QuestionTitle: question.Question,
			QuestionTypeCode: question.QuestionType?.Code ?? "SINGLE_CHOICE",
			AnswersJson: question.Answers,
			CorrectAnswer: question.CorrectAnswer,
			Explanation: question.Explanation,
			Expression: question.KnowledgeMeaning?.KnowledgeUnit?.Expression,
			Reading: question.KnowledgeMeaning?.KnowledgeUnit?.Reading,
			Meaning: question.KnowledgeMeaning?.Meaning);

		return JsonSerializer.Serialize(snapshot, JsonOptions);
	}

	public static string? ExtractOovText(string? sentence, int? startCharIndex, int? endCharIndex)
	{
		if (string.IsNullOrEmpty(sentence) ||
			!startCharIndex.HasValue ||
			!endCharIndex.HasValue ||
			startCharIndex.Value < 0 ||
			endCharIndex.Value <= startCharIndex.Value ||
			endCharIndex.Value > sentence.Length)
		{
			return null;
		}

		return sentence.Substring(startCharIndex.Value, endCharIndex.Value - startCharIndex.Value);
	}

	public static LearningUnit? FindMatchingLearningUnitForOov(
		TranscriptSegment? segment,
		int? startCharIndex,
		int? endCharIndex,
		string? selectedOovText = null)
	{
		if (segment is null || segment.LearningUnits is null || segment.LearningUnits.Count == 0)
		{
			return null;
		}

		var sentence = segment.Text ?? string.Empty;
		var oovText = !string.IsNullOrWhiteSpace(selectedOovText)
			? selectedOovText.Trim()
			: ExtractOovText(sentence, startCharIndex, endCharIndex)?.Trim();

		var orderedTokens = segment.Bunsetsus?
			.SelectMany(b => b.Tokens)
			.OrderBy(t => t.Sequence)
			.ToList() ?? [];

		LearningUnit? bestMatch = null;
		double bestScore = 0;

		if (startCharIndex.HasValue && endCharIndex.HasValue && endCharIndex.Value > startCharIndex.Value)
		{
			int targetStart = startCharIndex.Value;
			int targetEnd = endCharIndex.Value;

			foreach (var lu in segment.LearningUnits)
			{
				if (lu.KnowledgeMeaning?.KnowledgeUnit is null)
				{
					continue;
				}

				double candidateScore = 0;

				// 1. Direct half-open char-offset match (KnowledgeMatching units)
				if (lu.StartTokenIndex >= 0 &&
					lu.EndTokenIndex > lu.StartTokenIndex &&
					lu.EndTokenIndex <= sentence.Length)
				{
					candidateScore = Math.Max(
						candidateScore,
						ComputeSpanIoU(lu.StartTokenIndex, lu.EndTokenIndex, targetStart, targetEnd));
				}

				// 2. Token-indexed match mapped via Token.StartIndex..EndIndex (CuratorDefine units)
				if (orderedTokens.Count > 0 &&
					lu.StartTokenIndex >= 0 &&
					lu.EndTokenIndex >= lu.StartTokenIndex &&
					lu.EndTokenIndex < orderedTokens.Count)
				{
					int tokCharStart = orderedTokens[lu.StartTokenIndex].StartIndex;
					int tokCharEnd = orderedTokens[lu.EndTokenIndex].EndIndex;
					if (tokCharEnd > tokCharStart)
					{
						candidateScore = Math.Max(
							candidateScore,
							ComputeSpanIoU(tokCharStart, tokCharEnd, targetStart, targetEnd));
					}
				}

				if (candidateScore >= 0.5 && candidateScore > bestScore)
				{
					bestScore = candidateScore;
					bestMatch = lu;
				}
			}
		}

		if (bestMatch is not null)
		{
			return bestMatch;
		}

		// 3. Fallback: exact Expression match against reported OOV surface text
		if (!string.IsNullOrWhiteSpace(oovText))
		{
			return segment.LearningUnits.FirstOrDefault(lu =>
				string.Equals(
					lu.KnowledgeMeaning?.KnowledgeUnit?.Expression?.Trim(),
					oovText,
					StringComparison.Ordinal));
		}

		return null;
	}

	private static double ComputeSpanIoU(int startA, int endA, int startB, int endB)
	{
		int overlap = Math.Min(endA, endB) - Math.Max(startA, startB);
		if (overlap <= 0)
		{
			return 0;
		}

		int union = Math.Max(endA, endB) - Math.Min(startA, startB);
		return union > 0 ? (double)overlap / union : 0;
	}

	public static string SerializeTranscriptSegment(
		TranscriptSegment segment,
		int? startCharIndex = null,
		int? endCharIndex = null)
	{
		var video = segment.Transcript?.Video;
		var selectedOovText = ExtractOovText(segment.Text, startCharIndex, endCharIndex);
		var snapshot = new CurationTargetSnapshotDto(
			TargetType: nameof(TargetType.TranscriptSegment),
			VideoId: video?.Id.Value,
			VideoTitle: video?.Title ?? "Video bài học",
			StartTimeSeconds: segment.StartTime.TotalSeconds,
			EndTimeSeconds: segment.EndTime.TotalSeconds,
			OriginalSentenceJp: segment.Text,
			SubtitleVn: segment.Subtitle?.Text,
			TargetToken: selectedOovText,
			StartTokenIndex: startCharIndex,
			EndTokenIndex: endCharIndex,
			StartCharIndex: startCharIndex,
			EndCharIndex: endCharIndex,
			SelectedOovText: selectedOovText,
			VideoSourceType: video?.SourceType.ToString());

		return JsonSerializer.Serialize(snapshot, JsonOptions);
	}

	public static string SerializeLearningUnit(LearningUnit learningUnit)
	{
		var segment = learningUnit.TranscriptSegment;
		var video = segment?.Transcript?.Video;
		var expression = learningUnit.KnowledgeMeaning?.KnowledgeUnit?.Expression;
		var snapshot = new CurationTargetSnapshotDto(
			TargetType: nameof(TargetType.LearningUnit),
			VideoId: video?.Id.Value,
			VideoTitle: video?.Title ?? "Video bài học",
			StartTimeSeconds: segment?.StartTime.TotalSeconds,
			EndTimeSeconds: segment?.EndTime.TotalSeconds,
			OriginalSentenceJp: segment?.Text,
			SubtitleVn: segment?.Subtitle?.Text,
			TargetToken: expression,
			StartTokenIndex: learningUnit.StartTokenIndex,
			EndTokenIndex: learningUnit.EndTokenIndex,
			Expression: expression,
			Reading: learningUnit.KnowledgeMeaning?.KnowledgeUnit?.Reading,
			Meaning: learningUnit.KnowledgeMeaning?.Meaning,
			VideoSourceType: video?.SourceType.ToString());

		return JsonSerializer.Serialize(snapshot, JsonOptions);
	}

	public static CurationTargetSnapshotDto? DeserializeSnapshot(string? json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return null;
		}

		try
		{
			return JsonSerializer.Deserialize<CurationTargetSnapshotDto>(json, JsonOptions);
		}
		catch
		{
			return null;
		}
	}

	public static string FormatTimestamp(double totalSeconds)
	{
		var clampedSeconds = Math.Max(0, totalSeconds);
		int mins = (int)(clampedSeconds / 60);
		int secs = (int)(clampedSeconds % 60);
		return $"{mins:D2}:{secs:D2}";
	}

	public static string? FormatSnapshotTitle(TargetType targetType, CurationTargetSnapshotDto? snapshot)
	{
		if (snapshot is null)
		{
			return null;
		}

		switch (targetType)
		{
			case TargetType.BankQuestion:
				return !string.IsNullOrWhiteSpace(snapshot.QuestionTitle)
					? $"Câu hỏi: {snapshot.QuestionTitle}"
					: null;

			case TargetType.TranscriptSegment:
			{
				var videoTitle = snapshot.VideoTitle ?? "Video bài học";
				var timestamp = FormatTimestamp(snapshot.StartTimeSeconds ?? 0);
				var oovText = snapshot.SelectedOovText ?? snapshot.TargetToken;
				if (!string.IsNullOrWhiteSpace(oovText))
				{
					return $"Video: {videoTitle} ({timestamp}) - OOV 「{oovText}」";
				}

				return !string.IsNullOrWhiteSpace(snapshot.VideoTitle)
					? $"Video: {videoTitle} ({timestamp})"
					: null;
			}

			case TargetType.LearningUnit:
			{
				var expression = snapshot.Expression ?? snapshot.TargetToken;
				if (!string.IsNullOrWhiteSpace(expression))
				{
					return !string.IsNullOrWhiteSpace(snapshot.VideoTitle)
						? $"Video: {snapshot.VideoTitle} - Cụm 「{expression}」"
						: $"Đơn vị học: 「{expression}」";
				}

				return snapshot.VideoTitle;
			}

			default:
				return !string.IsNullOrWhiteSpace(snapshot.QuestionTitle)
					? $"Câu hỏi: {snapshot.QuestionTitle}"
					: snapshot.VideoTitle;
		}
	}

	public static async Task<(string? VideoUrl, string? ThumbnailUrl)> ResolveVideoMediaUrlsAsync(
		Video? video,
		IStorageService storageService,
		bool includeThumbnail,
		CancellationToken cancellationToken)
	{
		if (video is null)
		{
			return (null, null);
		}

		string? videoUrl = null;
		var videoAsset = video.MediaAssets
			.FirstOrDefault(a => a.AssetType == MediaAssetType.Video && !string.IsNullOrEmpty(a.StorageKey));

		if (videoAsset?.StorageKey is not null)
		{
			try
			{
				videoUrl = await storageService.CreatePresignedReadUrlAsync(videoAsset.StorageKey, cancellationToken);
			}
			catch
			{
				// Fallback to null when presigned URL generation fails
			}
		}
		else
		{
			videoUrl = video.SourceUrl;
		}

		string? thumbnailUrl = null;
		if (includeThumbnail)
		{
			var thumbnailAsset = video.MediaAssets
				.FirstOrDefault(a => a.AssetType == MediaAssetType.Thumbnail && !string.IsNullOrEmpty(a.StorageKey));

			if (thumbnailAsset?.StorageKey is not null)
			{
				try
				{
					thumbnailUrl = await storageService.CreatePresignedReadUrlAsync(thumbnailAsset.StorageKey, cancellationToken);
				}
				catch
				{
					// Non-critical thumbnail resolution
				}
			}
		}

		return (videoUrl, thumbnailUrl);
	}
}
