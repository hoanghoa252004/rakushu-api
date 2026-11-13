using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Subtitle;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Transcript.TranslateTranscript;

public sealed class TranslateTranscriptHandler
	: IRequestHandler<TranslateTranscriptCommand, Result>
{
	private const string SourceLanguageCode = "ja";
	private const string TargetLanguageCode = "vi";

	// Tổng số ký tự tối đa để sử dụng Realtime Translation.
	private const int RealtimeCharacterThreshold = 10000;

	private readonly IVideoRepository _videoRepository;
	private readonly IStorageService _storageService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ITranslateService _translateService;

	public TranslateTranscriptHandler(
		IStorageService storageService,
		IVideoRepository videoRepository,
		IUnitOfWork unitOfWork,
		ITranslateService translateService)
	{
		_storageService = storageService;
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
		_translateService = translateService;
	}

	public async Task<Result> Handle(
		TranslateTranscriptCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var videos = await _videoRepository
				.GetProcessingVideosWithReviewedTranscriptForTranslateAsync(
					10,
					cancellationToken);

			foreach (var video in videos)
			{
				var transcript = video.Transcript;

				if (transcript is null ||
					transcript.TranscriptSegments.Count == 0)
				{
					continue;
				}

				var segments = transcript.TranscriptSegments
					.OrderBy(segment => segment.Sequence)
					.ToList();

				// Chuẩn bị nội dung dịch.
				var segmentTexts = segments
					.Select(segment => segment.Text)
					.ToList();

				var totalCharacters = CountCharacters(segmentTexts);

				// Xác định phương thức dịch TRƯỚC khi tìm Job.
				if (totalCharacters <= RealtimeCharacterThreshold)
				{
					await TranslateRealtimeAsync(
						segments,
						segmentTexts,
						cancellationToken);

					continue;
				}

				// Chỉ những transcript vượt ngưỡng mới sử dụng Batch Job.
				await TranslateBatchAsync(
					transcript.Id.ToString(),
					segments,
					segmentTexts,
					cancellationToken);
			}

			return Result.Success();

		}, cancellationToken);
	}

	#region Translation Strategy

	private static int CountCharacters(
		IReadOnlyList<string> segmentTexts)
	{
		return segmentTexts.Sum(text => text.Length);
	}

	#endregion

	#region Realtime Translation

	private async Task TranslateRealtimeAsync(
		IReadOnlyList<Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment> segments,
		IReadOnlyList<string> segmentTexts,
		CancellationToken cancellationToken)
	{
		// Ghép các segment thành một nội dung duy nhất.
		// Giữ nguyên newline để phân biệt từng segment.
		var content = string.Join("\n", segmentTexts);

		// Gọi AWS Translate Realtime API.
		var translatedContent = await _translateService.TranslateTextAsync(
			content,
			SourceLanguageCode,
			TargetLanguageCode,
			cancellationToken);

		// Parse kết quả thành từng dòng.
		var translatedLines = ParseTranslatedLines(translatedContent);

		// Tạo subtitle theo thứ tự segment.
		await ProcessTranslatedLinesAsync(
			segments,
			translatedLines,
			cancellationToken);
	}

	#endregion

	#region Batch Translation

	private async Task TranslateBatchAsync(
		string transcriptId,
		IReadOnlyList<Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment> segments,
		IReadOnlyList<string> segmentTexts,
		CancellationToken cancellationToken)
	{
		var jobName = $"rakushu-transcript-{transcriptId}";

		// Chỉ tìm Job khi transcript thuộc Batch Translation.
		var existingJob =
			await _translateService.FindBatchTranslationJobByNameAsync(
				jobName,
				cancellationToken);

		if (existingJob is null)
		{
			await CreateTranslationJobAsync(
				transcriptId,
				segmentTexts,
				jobName,
				cancellationToken);

			return;
		}

		var job = await _translateService.GetBatchTranslationJobAsync(
			existingJob.JobId,
			cancellationToken);

		switch (job.Status)
		{
			case "SUBMITTED":
			case "IN_PROGRESS":
			case "STOP_REQUESTED":

				// Job chưa hoàn thành.
				return;

			case "COMPLETED":

				await ProcessCompletedTranslationAsync(
					transcriptId,
					segments,
					job,
					cancellationToken);

				return;

			case "FAILED":
			case "STOPPED":

				// Chưa tự retry để tránh tạo Job lặp.
				return;

			default:
				return;
		}
	}

	private async Task CreateTranslationJobAsync(
		string transcriptId,
		IReadOnlyList<string> segmentTexts,
		string jobName,
		CancellationToken cancellationToken)
	{
		if (segmentTexts.Count == 0)
		{
			throw new InvalidOperationException(
				$"Không có segment để dịch cho transcript {transcriptId}.");
		}

		var content = string.Join("\n", segmentTexts);

		var inputPrefix = $"translation/input/{transcriptId}/";
		var inputKey = $"{inputPrefix}source.txt";

		var outputPrefix = $"translation/output/{transcriptId}/";

		await _storageService.UploadTextFileAsync(
			inputKey,
			content,
			cancellationToken);

		var inputS3Uri = _storageService.GetS3MediaUri(inputPrefix);
		var outputS3Uri = _storageService.GetS3MediaUri(outputPrefix);

		await _translateService.StartBatchTranslationJobAsync(
			inputS3Uri,
			outputS3Uri,
			SourceLanguageCode,
			TargetLanguageCode,
			jobName,
			cancellationToken);
	}

	private async Task ProcessCompletedTranslationAsync(
		string transcriptId,
		IReadOnlyList<Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment> segments,
		TranslationJobStatusDto job,
		CancellationToken cancellationToken)
	{
		var outputKey = await _storageService.FindTranslatedTextFileKeyAsync(
			$"translation/output/{transcriptId}/",
			cancellationToken);

		if (string.IsNullOrWhiteSpace(outputKey))
		{
			throw new InvalidOperationException(
				$"Không tìm thấy file output cho transcript {transcriptId}.");
		}

		var translatedContent = await _storageService.ReadTextFileAsync(
			outputKey,
			cancellationToken);

		var translatedLines = ParseTranslatedLines(translatedContent);

		await ProcessTranslatedLinesAsync(
			segments,
			translatedLines,
			cancellationToken);
	}

	#endregion

	#region Shared Subtitle Processing

	private static List<string> ParseTranslatedLines(
		string translatedContent)
	{
		var translatedLines = translatedContent
			.Replace("\r\n", "\n")
			.Split('\n')
			.Select(line => line.TrimEnd('\r'))
			.ToList();

		// Chỉ bỏ newline cuối file.
		// Không loại bỏ dòng trống ở giữa.
		if (translatedLines.Count > 0 &&
			translatedLines[^1].Length == 0 &&
			translatedContent.EndsWith('\n'))
		{
			translatedLines.RemoveAt(translatedLines.Count - 1);
		}

		return translatedLines;
	}

	private async Task ProcessTranslatedLinesAsync(
		IReadOnlyList<Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment> segments,
		IReadOnlyList<string> translatedLines,
		CancellationToken cancellationToken)
	{
		if (translatedLines.Count != segments.Count)
		{
			throw new InvalidOperationException(
				$"Số dòng dịch không khớp số segment. " +
				$"Segments={segments.Count}, " +
				$"TranslatedLines={translatedLines.Count}.");
		}

		for (var index = 0; index < segments.Count; index++)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var segment = segments[index];
			var translatedText = translatedLines[index];

			if (string.IsNullOrWhiteSpace(translatedText))
			{
				throw new InvalidOperationException(
					$"Bản dịch rỗng tại segment sequence {segment.Sequence}.");
			}

			// Không tạo lại subtitle nếu đã có subtitle tiếng Việt.
			if (segment.Subtitle is not null &&
				segment.Subtitle.LanguageCode.Equals(LanguageCode.VN))
			{
				continue;
			}

			var subtitleResult = segment.AddSubtitle(
				LanguageCode.VN,
				translatedText);

			if (subtitleResult.IsFailure)
			{
				throw new InvalidOperationException(
					$"Không thể tạo subtitle cho segment sequence {segment.Sequence}.");
			}
		}
	}

	#endregion
}