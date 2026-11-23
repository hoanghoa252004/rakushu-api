using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation;

public static class CurationContextHelper
{
	public const string OovCategory = "Oov";
	public const string WrongSpeakerCategory = "WrongSpeaker";
	public const string ShadowingReevaluationCategory = "ShadowingReevaluation";

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	private static readonly Regex BrTagRegex = new(@"<\s*br\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex BlockCloseTagRegex = new(@"</\s*(p|div|li|h[1-6]|blockquote)\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex ListItemOpenTagRegex = new(@"<\s*li[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex AnyHtmlTagRegex = new(@"<[^>]+>", RegexOptions.Compiled);
	private static readonly Regex NbspEntityRegex = new(@"&nbsp;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex AmpEntityRegex = new(@"&amp;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex LtEntityRegex = new(@"&lt;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex GtEntityRegex = new(@"&gt;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex QuotEntityRegex = new(@"&quot;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex AposEntityRegex = new(@"&#39;|&apos;", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex TrailingSpacesBeforeNewlineRegex = new(@"[ \t]+\n", RegexOptions.Compiled);
	private static readonly Regex ExcessNewlinesRegex = new(@"\n{3,}", RegexOptions.Compiled);

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
			Meaning: question.KnowledgeMeaning?.Meaning,
			MediaType: question.MediaType,
			MediaUrl: question.MediaUrl,
			MediaMetadata: question.MediaMetadata);

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
		var transcriptSpeaker = segment.TranscriptSpeaker;
		var speaker = transcriptSpeaker?.Speaker;
		var selectedOovText = ExtractOovText(segment.Text, startCharIndex, endCharIndex);
		var matchedOovUnit = startCharIndex.HasValue && endCharIndex.HasValue
			? FindMatchingLearningUnitForOov(segment, startCharIndex, endCharIndex, selectedOovText)
			: null;
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
			Expression: matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Expression,
			Reading: matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Reading,
			Meaning: matchedOovUnit?.KnowledgeMeaning?.Meaning,
			StartCharIndex: startCharIndex,
			EndCharIndex: endCharIndex,
			SelectedOovText: selectedOovText,
			VideoSourceType: video?.SourceType.ToString(),
			TranscriptSpeakerId: segment.TranscriptSpeakerId?.Value,
			SpeakerId: transcriptSpeaker?.SpeakerId?.Value,
			SpeakerLabel: transcriptSpeaker?.Label,
			SpeakerName: speaker?.Name,
			SpeakerJapaneseName: speaker?.JapaneseName,
			SpeakerThemeColor: speaker?.ThemeColor);

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

	public static string SerializeShadowingAttempt(ShadowingAttemptAssessmentContextModel ctx)
	{
		var context = BuildShadowingAttemptContext(ctx);
		var baseline = context.AutomatedAssessment ?? context.CuratorAssessment;

		var snapshot = new CurationTargetSnapshotDto(
			TargetType: nameof(TargetType.ShadowingAttempt),
			VideoTitle: ctx.VideoTitle,
			ShadowingSessionId: ctx.SessionId,
			ShadowingSessionTitle: ctx.SessionTitle,
			ShadowingMaterialName: ctx.MaterialName,
			ShadowingMaterialJapaneseName: ctx.MaterialJapaneseName,
			ShadowingRoleName: ctx.RoleName,
			ShadowingLearnerName: ctx.LearnerName,
			ShadowingOverallScore: baseline?.OverallScore,
			ShadowingAccuracyScore: baseline?.AccuracyScore,
			ShadowingPitchSimilarityScore: baseline?.PitchSimilarityScore,
			ShadowingOverallFeedback: baseline?.OverallFeedback,
			ShadowingAssessedLineCount: context.AssessedLinesCount,
			ShadowingSkippedLineCount: context.SkippedLinesCount);

		return JsonSerializer.Serialize(snapshot, JsonOptions);
	}

	public static ShadowingAttemptContextDto BuildShadowingAttemptContext(ShadowingAttemptAssessmentContextModel ctx)
	{
		var automated = ctx.Assessments
			.Where(a => a.Type == ShadowingAssessmentType.Automated)
			.OrderByDescending(a => a.CreatedAt)
			.FirstOrDefault();

		var curator = ctx.Assessments
			.Where(a => a.Type == ShadowingAssessmentType.CuratorReevaluation)
			.OrderByDescending(a => a.CreatedAt)
			.FirstOrDefault();

		var automatedSummary = automated is not null ? BuildAssessmentSummary(automated) : null;
		var curatorSummary = curator is not null ? BuildAssessmentSummary(curator) : null;

		int assessedLinesCount = ctx.Lines.Count(l => !l.IsSkipped);
		int skippedLinesCount = ctx.Lines.Count(l => l.IsSkipped);

		return new ShadowingAttemptContextDto(
			ctx.AttemptId,
			ctx.SessionId,
			ctx.SessionTitle,
			ctx.MaterialName,
			ctx.MaterialJapaneseName,
			ctx.VideoTitle,
			ctx.RoleName,
			ctx.LearnerName,
			ctx.AttemptStatus.ToString(),
			ctx.Lines.Count,
			assessedLinesCount,
			skippedLinesCount,
			curatorSummary is not null,
			automatedSummary,
			curatorSummary,
			ctx.StartedAt,
			ctx.CompletedAt);
	}

	public static string MapLearnerIssueStatus(Report? report)
	{
		return report switch
		{
			null => "Pending",
			{ Status: ReportStatus.Pending } => "Pending",
			{ Status: ReportStatus.InReview } => "Investigating",
			{ Status: ReportStatus.Resolved } => "Resolved",
			{ Status: ReportStatus.Dismissed } => "Dismissed",
			_ => report.Status.ToString()
		};
	}

	private static ShadowingAssessmentSummaryDto BuildAssessmentSummary(ShadowingAssessmentContextModel assessment)
	{
		var accScores = assessment.Details
			.Where(d => d.AccuracyScore.HasValue)
			.Select(d => d.AccuracyScore!.Value)
			.ToList();
		var pitchScores = assessment.Details
			.Where(d => d.PitchSimilarityScore.HasValue)
			.Select(d => d.PitchSimilarityScore!.Value)
			.ToList();

		decimal? avgAcc = accScores.Count > 0 ? Math.Round(accScores.Average(), 1) : null;
		decimal? avgPitch = pitchScores.Count > 0 ? Math.Round(pitchScores.Average(), 1) : null;
		decimal? overall = assessment.OverallScore.HasValue
			? Math.Round(assessment.OverallScore.Value, 1)
			: assessment.Details.Any(d => d.AccuracyScore.HasValue || d.PitchSimilarityScore.HasValue)
				? Math.Round(
					assessment.Details
						.Where(d => d.AccuracyScore.HasValue || d.PitchSimilarityScore.HasValue)
						.Average(d => ((d.AccuracyScore ?? 0m) + (d.PitchSimilarityScore ?? 0m)) / 2m),
					1)
				: null;

		return new ShadowingAssessmentSummaryDto(
			assessment.AssessmentId,
			assessment.Type.ToString(),
			overall,
			avgAcc,
			avgPitch,
			assessment.OverallFeedback,
			assessment.CreatedAt);
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

			case TargetType.ShadowingAttempt:
			{
				var materialName = snapshot.ShadowingMaterialName ?? snapshot.VideoTitle;
				if (string.IsNullOrWhiteSpace(materialName))
				{
					return "Luyện Shadowing";
				}

				var roleSuffix = !string.IsNullOrWhiteSpace(snapshot.ShadowingRoleName)
					? $" ({snapshot.ShadowingRoleName})"
					: string.Empty;
				var scoreSuffix = snapshot.ShadowingOverallScore.HasValue
					? $" — {snapshot.ShadowingOverallScore.Value.ToString("0.#", CultureInfo.InvariantCulture)} điểm"
					: string.Empty;
				return $"Shadowing: {materialName}{roleSuffix}{scoreSuffix}";
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

	public static async Task<string?> ResolveSpeakerAvatarUrlAsync(
		string? avatarKey,
		IStorageService storageService,
		CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(avatarKey))
		{
			return null;
		}

		try
		{
			return await storageService.CreatePresignedReadUrlAsync(avatarKey, cancellationToken);
		}
		catch
		{
			return null;
		}
	}

	public static CurationTargetSnapshotDto? ResolveCanonicalSnapshot(Report report)
	{
		var rawSnapshotJson = !string.IsNullOrWhiteSpace(report.TargetSnapshot)
			? report.TargetSnapshot
			: report.Details
				.OrderBy(d => d.CreatedAt)
				.Select(d => d.TargetSnapshot)
				.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

		return DeserializeSnapshot(rawSnapshotJson);
	}

	public static string StripHtmlToPlainText(string? raw)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return string.Empty;
		}

		var text = BrTagRegex.Replace(raw, "\n");
		text = BlockCloseTagRegex.Replace(text, "\n");
		text = ListItemOpenTagRegex.Replace(text, "• ");
		text = AnyHtmlTagRegex.Replace(text, string.Empty);
		text = NbspEntityRegex.Replace(text, " ");
		text = AmpEntityRegex.Replace(text, "&");
		text = LtEntityRegex.Replace(text, "<");
		text = GtEntityRegex.Replace(text, ">");
		text = QuotEntityRegex.Replace(text, "\"");
		text = AposEntityRegex.Replace(text, "'");
		text = TrailingSpacesBeforeNewlineRegex.Replace(text, "\n");
		text = ExcessNewlinesRegex.Replace(text, "\n\n");
		return text.Trim();
	}

	public static bool HasBankQuestionChangedOrArchived(
		Report report,
		CurationTargetSnapshotDto? snapshot,
		BankQuestion? liveQuestion)
	{
		bool hasQuestionSnap = HasBankQuestionSnapshot(snapshot);
		if (liveQuestion is null)
		{
			return snapshot is not null;
		}

		if (!liveQuestion.IsActive)
		{
			return true;
		}

		if (hasQuestionSnap)
		{
			if (!string.Equals(
				(liveQuestion.Question ?? string.Empty).Trim(),
				(snapshot!.QuestionTitle ?? string.Empty).Trim(),
				StringComparison.Ordinal))
			{
				return true;
			}

			if (!AnswerOptionsEqual(
				ParseAnswerOptions(liveQuestion.Answers),
				ParseAnswerOptions(snapshot.AnswersJson)))
			{
				return true;
			}

			if (!string.Equals(
				(liveQuestion.CorrectAnswer ?? string.Empty).Trim(),
				(snapshot.CorrectAnswer ?? string.Empty).Trim(),
				StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			if (!string.Equals(
				StripHtmlToPlainText(liveQuestion.Explanation),
				StripHtmlToPlainText(snapshot.Explanation),
				StringComparison.Ordinal))
			{
				return true;
			}
		}

		bool hasMediaSnap = !string.IsNullOrWhiteSpace(snapshot?.MediaType);
		if (hasMediaSnap)
		{
			var liveMediaType = string.IsNullOrWhiteSpace(liveQuestion.MediaType) ? "NONE" : liveQuestion.MediaType.Trim();
			var snapMediaType = snapshot!.MediaType!.Trim();
			if (!string.Equals(liveMediaType, snapMediaType, StringComparison.OrdinalIgnoreCase)
				|| !string.Equals((liveQuestion.MediaUrl ?? string.Empty).Trim(), (snapshot.MediaUrl ?? string.Empty).Trim(), StringComparison.Ordinal)
				|| !string.Equals((liveQuestion.MediaMetadata ?? string.Empty).Trim(), (snapshot.MediaMetadata ?? string.Empty).Trim(), StringComparison.Ordinal))
			{
				return true;
			}
		}

		// Fallback for legacy/partial snapshots (e.g. missing questionTitle/answersJson/mediaType) or null snapshots:
		// any BankQuestion mutation updates UpdatedAt, so UpdatedAt > report.CreatedAt reliably detects edits.
		if (!(hasQuestionSnap && hasMediaSnap) && liveQuestion.UpdatedAt > report.CreatedAt)
		{
			return true;
		}

		return false;
	}

	public static bool HasTranscriptSegmentChangedOrRemoved(
		Report report,
		CurationTargetSnapshotDto? snapshot,
		TranscriptSegment? liveSegment)
	{
		bool hasVideoSnap = HasVideoSnapshot(snapshot);
		if (liveSegment is null)
		{
			return hasVideoSnap;
		}

		if (!hasVideoSnap)
		{
			return false;
		}

		bool hasFullVideoSnap = snapshot!.StartTimeSeconds.HasValue;

		if (snapshot.StartTimeSeconds.HasValue && snapshot.EndTimeSeconds.HasValue)
		{
			if (Math.Abs(liveSegment.StartTime.TotalSeconds - snapshot.StartTimeSeconds.Value) > 0.05
				|| Math.Abs(liveSegment.EndTime.TotalSeconds - snapshot.EndTimeSeconds.Value) > 0.05)
			{
				return true;
			}
		}

		if ((hasFullVideoSnap || snapshot.OriginalSentenceJp is not null)
			&& !string.Equals(
				(liveSegment.Text ?? string.Empty).Trim(),
				(snapshot.OriginalSentenceJp ?? string.Empty).Trim(),
				StringComparison.Ordinal))
		{
			return true;
		}

		if ((hasFullVideoSnap || snapshot.SubtitleVn is not null)
			&& !string.Equals(
				(liveSegment.Subtitle?.Text ?? string.Empty).Trim(),
				(snapshot.SubtitleVn ?? string.Empty).Trim(),
				StringComparison.Ordinal))
		{
			return true;
		}

		if (string.Equals(report.Category, OovCategory, StringComparison.OrdinalIgnoreCase))
		{
			int? startCharIndex = report.StartCharIndex ?? snapshot.StartCharIndex;
			int? endCharIndex = report.EndCharIndex ?? snapshot.EndCharIndex;
			var oovText = ExtractOovText(liveSegment.Text, startCharIndex, endCharIndex)
				?? snapshot.SelectedOovText;
			var matchedOovUnit = FindMatchingLearningUnitForOov(
				liveSegment,
				startCharIndex,
				endCharIndex,
				oovText);

			var liveExpr = (matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Expression ?? string.Empty).Trim();
			var liveReading = (matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Reading ?? string.Empty).Trim();
			var liveMeaning = (matchedOovUnit?.KnowledgeMeaning?.Meaning ?? string.Empty).Trim();

			if (!string.Equals(liveExpr, (snapshot.Expression ?? string.Empty).Trim(), StringComparison.Ordinal)
				|| !string.Equals(liveReading, (snapshot.Reading ?? string.Empty).Trim(), StringComparison.Ordinal)
				|| !string.Equals(liveMeaning, (snapshot.Meaning ?? string.Empty).Trim(), StringComparison.Ordinal))
			{
				return true;
			}
		}

		bool isWrongSpeaker = string.Equals(report.Category, WrongSpeakerCategory, StringComparison.OrdinalIgnoreCase);
		if (HasSegmentSpeakerChanged(snapshot, liveSegment, includeUnassignedToAssigned: isWrongSpeaker))
		{
			return true;
		}

		return false;
	}

	public static bool HasLearningUnitChangedOrRemoved(
		CurationTargetSnapshotDto? snapshot,
		LearningUnit? liveUnit)
	{
		bool hasVideoSnap = HasVideoSnapshot(snapshot);
		if (liveUnit is null)
		{
			return hasVideoSnap;
		}

		if (!hasVideoSnap)
		{
			return false;
		}

		bool hasFullVideoSnap = snapshot!.StartTimeSeconds.HasValue;
		var segment = liveUnit.TranscriptSegment;
		if (segment is not null && snapshot.StartTimeSeconds.HasValue && snapshot.EndTimeSeconds.HasValue)
		{
			if (Math.Abs(segment.StartTime.TotalSeconds - snapshot.StartTimeSeconds.Value) > 0.05
				|| Math.Abs(segment.EndTime.TotalSeconds - snapshot.EndTimeSeconds.Value) > 0.05)
			{
				return true;
			}
		}

		if ((hasFullVideoSnap || snapshot.OriginalSentenceJp is not null)
			&& !string.Equals(
				(segment?.Text ?? string.Empty).Trim(),
				(snapshot.OriginalSentenceJp ?? string.Empty).Trim(),
				StringComparison.Ordinal))
		{
			return true;
		}

		if ((hasFullVideoSnap || snapshot.SubtitleVn is not null)
			&& !string.Equals(
				(segment?.Subtitle?.Text ?? string.Empty).Trim(),
				(snapshot.SubtitleVn ?? string.Empty).Trim(),
				StringComparison.Ordinal))
		{
			return true;
		}

		if (snapshot.StartTokenIndex.HasValue && snapshot.EndTokenIndex.HasValue)
		{
			if (liveUnit.StartTokenIndex != snapshot.StartTokenIndex.Value
				|| liveUnit.EndTokenIndex != snapshot.EndTokenIndex.Value)
			{
				return true;
			}
		}

		var liveExpr = (liveUnit.KnowledgeMeaning?.KnowledgeUnit?.Expression ?? string.Empty).Trim();
		var snapExpr = (snapshot.Expression ?? snapshot.TargetToken ?? string.Empty).Trim();
		var liveReading = (liveUnit.KnowledgeMeaning?.KnowledgeUnit?.Reading ?? string.Empty).Trim();
		var snapReading = (snapshot.Reading ?? string.Empty).Trim();
		var liveMeaning = (liveUnit.KnowledgeMeaning?.Meaning ?? string.Empty).Trim();
		var snapMeaning = (snapshot.Meaning ?? string.Empty).Trim();

		bool exprChanged = (hasFullVideoSnap || snapshot.Expression is not null || snapshot.TargetToken is not null)
			&& !string.Equals(liveExpr, snapExpr, StringComparison.Ordinal);
		bool readingChanged = (hasFullVideoSnap || snapshot.Reading is not null)
			&& !string.Equals(liveReading, snapReading, StringComparison.Ordinal);
		bool meaningChanged = (hasFullVideoSnap || snapshot.Meaning is not null)
			&& !string.Equals(liveMeaning, snapMeaning, StringComparison.Ordinal);

		if (exprChanged || readingChanged || meaningChanged)
		{
			return true;
		}

		return false;
	}

	public static bool HasSegmentSpeakerChanged(
		CurationTargetSnapshotDto? snapshot,
		TranscriptSegment? liveSegment,
		bool includeUnassignedToAssigned = true)
	{
		if (snapshot is null || liveSegment is null)
		{
			return false;
		}

		if (!includeUnassignedToAssigned && !HasAnySpeakerMetadata(snapshot))
		{
			return false;
		}

		var snapKey = GetSpeakerComparisonKey(
			snapshot.TranscriptSpeakerId,
			snapshot.SpeakerId,
			snapshot.SpeakerLabel,
			snapshot.SpeakerName);

		var liveTs = liveSegment.TranscriptSpeaker;
		var liveKey = GetSpeakerComparisonKey(
			liveSegment.TranscriptSpeakerId?.Value,
			liveTs?.SpeakerId?.Value,
			liveTs?.Label,
			liveTs?.Speaker?.Name);

		return !string.Equals(snapKey, liveKey, StringComparison.Ordinal);
	}

	private static bool HasBankQuestionSnapshot(CurationTargetSnapshotDto? snapshot)
	{
		if (snapshot is null)
		{
			return false;
		}

		return !string.IsNullOrWhiteSpace(snapshot.QuestionTitle)
			|| ParseAnswerOptions(snapshot.AnswersJson).Count > 0
			|| !string.IsNullOrWhiteSpace(StripHtmlToPlainText(snapshot.Explanation));
	}

	private static bool HasVideoSnapshot(CurationTargetSnapshotDto? snapshot)
	{
		if (snapshot is null)
		{
			return false;
		}

		return !string.IsNullOrWhiteSpace(snapshot.OriginalSentenceJp)
			|| !string.IsNullOrWhiteSpace(snapshot.SubtitleVn)
			|| snapshot.StartTimeSeconds.HasValue
			|| !string.IsNullOrWhiteSpace(snapshot.Expression)
			|| !string.IsNullOrWhiteSpace(snapshot.TargetToken)
			|| HasAnySpeakerMetadata(snapshot);
	}

	private static bool HasAnySpeakerMetadata(CurationTargetSnapshotDto? snapshot)
	{
		if (snapshot is null)
		{
			return false;
		}

		return snapshot.TranscriptSpeakerId.HasValue
			|| snapshot.SpeakerId.HasValue
			|| !string.IsNullOrWhiteSpace(snapshot.SpeakerLabel)
			|| !string.IsNullOrWhiteSpace(snapshot.SpeakerName);
	}

	private static List<(string Id, string Text)> ParseAnswerOptions(string? answersJson)
	{
		var result = new List<(string Id, string Text)>();
		if (string.IsNullOrWhiteSpace(answersJson))
		{
			return result;
		}

		try
		{
			using var doc = JsonDocument.Parse(answersJson);
			if (doc.RootElement.ValueKind == JsonValueKind.Array)
			{
				int idx = 0;
				foreach (var elem in doc.RootElement.EnumerateArray())
				{
					string defaultId = ((char)('A' + idx)).ToString();
					if (elem.ValueKind == JsonValueKind.Object)
					{
						string ansId = defaultId;
						if (TryGetJsonPropertyCaseInsensitive(elem, "id", out var idProp))
						{
							var rawId = JsonElementToString(idProp);
							if (!string.IsNullOrEmpty(rawId))
							{
								ansId = rawId;
							}
						}

						string ansText = string.Empty;
						if (TryGetJsonPropertyCaseInsensitive(elem, "text", out var textProp))
						{
							ansText = JsonElementToString(textProp);
						}

						result.Add((ansId, ansText));
					}
					else
					{
						result.Add((defaultId, JsonElementToString(elem)));
					}

					idx++;
				}

				return result;
			}
		}
		catch
		{
			// Fallback to raw single answer string matching FE parseAnswersList behavior
		}

		result.Add(("A", answersJson));
		return result;
	}

	private static bool TryGetJsonPropertyCaseInsensitive(
		JsonElement element,
		string propertyName,
		out JsonElement value)
	{
		foreach (var prop in element.EnumerateObject())
		{
			if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
			{
				value = prop.Value;
				return true;
			}
		}

		value = default;
		return false;
	}

	private static string JsonElementToString(JsonElement element)
	{
		return element.ValueKind switch
		{
			JsonValueKind.String => element.GetString() ?? string.Empty,
			JsonValueKind.Null or JsonValueKind.Undefined => string.Empty,
			_ => element.GetRawText()
		};
	}

	private static bool AnswerOptionsEqual(
		List<(string Id, string Text)> left,
		List<(string Id, string Text)> right)
	{
		if (left.Count != right.Count)
		{
			return false;
		}

		for (int i = 0; i < left.Count; i++)
		{
			if (!string.Equals(left[i].Id, right[i].Id, StringComparison.Ordinal)
				|| !string.Equals(left[i].Text, right[i].Text, StringComparison.Ordinal))
			{
				return false;
			}
		}

		return true;
	}

	private static string GetSpeakerComparisonKey(
		Guid? transcriptSpeakerId,
		Guid? speakerId,
		string? speakerLabel,
		string? speakerName)
	{
		if (speakerId.HasValue)
		{
			return $"sp:{speakerId.Value:D}";
		}

		var trimmedName = speakerName?.Trim();
		if (!string.IsNullOrEmpty(trimmedName)
			&& !trimmedName.Equals("Chưa gán vai", StringComparison.OrdinalIgnoreCase)
			&& !trimmedName.StartsWith("spk_", StringComparison.OrdinalIgnoreCase))
		{
			return $"name:{trimmedName.ToLowerInvariant()}";
		}

		var trimmedLabel = speakerLabel?.Trim().ToLowerInvariant() ?? string.Empty;
		if (transcriptSpeakerId.HasValue)
		{
			return $"ts:{transcriptSpeakerId.Value:D}:{trimmedLabel}";
		}

		if (!string.IsNullOrEmpty(trimmedLabel))
		{
			return $"lbl:{trimmedLabel}";
		}

		return "unassigned";
	}
}
