using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;
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

	// Tổng số byte (UTF-8) tối đa để sử dụng Realtime Translation (chia chunk), vượt ngưỡng dùng Batch Job.
	private const int RealtimeByteThreshold = 100_000;

	// AWS Translate realtime giới hạn 10000 byte / request, chia chunk ~8000 byte để an toàn.
	private const int ChunkByteLimit = 8000;

	private readonly IVideoRepository _videoRepository;
	private readonly IStorageService _storageService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ITranslateService _translateService;
	private readonly IProcessingJobLogService _jobLogService;

	public TranslateTranscriptHandler(
		IStorageService storageService,
		IVideoRepository videoRepository,
		IUnitOfWork unitOfWork,
		ITranslateService translateService,
		IProcessingJobLogService jobLogService)
	{
		_jobLogService = jobLogService;
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

				var totalBytes = CountBytes(segmentTexts);

				// Xác định phương thức dịch TRƯỚC khi tìm Job.
				if (totalBytes <= RealtimeByteThreshold)
				{
					await TranslateRealtimeAsync(
						video.Id.Value,
						segments,
						segmentTexts,
						cancellationToken);

					continue;
				}

				// Chỉ những transcript vượt ngưỡng mới sử dụng Batch Job.
				await TranslateBatchAsync(
					video.Id.Value,
					transcript.Id.ToString(),
					segments,
					segmentTexts,
					cancellationToken);
			}

			return Result.Success();

		}, cancellationToken);
	}

	#region Translation Strategy

	private static int CountBytes(
		IReadOnlyList<string> segmentTexts)
	{
		// Tính cả ký tự newline nối giữa các segment.
		return segmentTexts.Sum(text => System.Text.Encoding.UTF8.GetByteCount(text))
			+ Math.Max(0, segmentTexts.Count - 1);
	}

	/// <summary>
	/// Chia segment (đã sắp theo Sequence) thành các chunk liên tiếp, mỗi chunk tối đa ChunkByteLimit byte.
	/// Không cắt ngang segment nên thứ tự và số dòng của mỗi chunk được giữ nguyên.
	/// </summary>
	private static List<(int Start, int Count)> BuildChunks(
		IReadOnlyList<string> segmentTexts)
	{
		var chunks = new List<(int Start, int Count)>();
		var start = 0;
		var bytes = 0;

		for (var index = 0; index < segmentTexts.Count; index++)
		{
			var size = System.Text.Encoding.UTF8.GetByteCount(segmentTexts[index]) + 1;

			if (index > start && bytes + size > ChunkByteLimit)
			{
				chunks.Add((start, index - start));
				start = index;
				bytes = 0;
			}

			bytes += size;
		}

		if (start < segmentTexts.Count)
			chunks.Add((start, segmentTexts.Count - start));

		return chunks;
	}

	#endregion

	#region Realtime Translation

	/// <summary>
	/// Dịch từng chunk tuần tự theo thứ tự segment. Mỗi chunk là một job log Translation;
	/// chunk đầu nối với Transcription, các chunk sau có ParentJobLogId là chunk trước đó.
	/// </summary>
	private async Task TranslateRealtimeAsync(
		Guid videoId,
		IReadOnlyList<Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment> segments,
		IReadOnlyList<string> segmentTexts,
		CancellationToken cancellationToken)
	{
		var chunks = BuildChunks(segmentTexts);
		var translatedLines = new List<string>(segments.Count);
		Guid? previousLogId = null;

		for (var chunkIndex = 0; chunkIndex < chunks.Count; chunkIndex++)
		{
			var (start, count) = chunks[chunkIndex];
			var chunkTexts = segmentTexts.Skip(start).Take(count).ToList();

			// Giữ nguyên newline để phân biệt từng segment.
			var content = string.Join("\n", chunkTexts);

			var chunkLog = await _jobLogService.StartAsync(
				ProcessingJobType.Translation,
				ProcessingJobTargetType.Video,
				videoId,
				ProcessingJobServices.AwsTranslate,
				content,
				cancellationToken,
				previousLogId);

			await _jobLogService.SetJobLabelAsync(
				chunkLog.Id,
				$"chunk {chunkIndex + 1}/{chunks.Count} (segment {segments[start].Sequence}-{segments[start + count - 1].Sequence})",
				cancellationToken);

			List<string> chunkLines;
			string translatedContent;
			try
			{
				translatedContent = await _translateService.TranslateTextAsync(
					content,
					SourceLanguageCode,
					TargetLanguageCode,
					cancellationToken);

				chunkLines = ParseTranslatedLines(translatedContent);

				if (chunkLines.Count != count)
				{
					throw new InvalidOperationException(
						$"Số dòng dịch không khớp số segment ở chunk {chunkIndex + 1}. " +
						$"Segments={count}, TranslatedLines={chunkLines.Count}.");
				}
			}
			catch (Exception ex)
			{
				await _jobLogService.FailAsync(chunkLog.Id, ex.Message, CancellationToken.None);
				throw;
			}

			await _jobLogService.CompleteAsync(chunkLog.Id, translatedContent, cancellationToken);

			translatedLines.AddRange(chunkLines);
			previousLogId = chunkLog.Id;
		}

		// Tạo subtitle theo thứ tự segment.
		await ProcessTranslatedLinesAsync(
			segments,
			translatedLines,
			cancellationToken);
	}

	#endregion

	#region Batch Translation

	private async Task TranslateBatchAsync(
		Guid videoId,
		string transcriptId,
		IReadOnlyList<Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment> segments,
		IReadOnlyList<string> segmentTexts,
		CancellationToken cancellationToken)
	{
		var jobLog = await _jobLogService.GetOrStartAsync(
			ProcessingJobType.Translation,
			ProcessingJobTargetType.Video,
			videoId,
			ProcessingJobServices.AwsTranslate,
			$"translation/input/{transcriptId}/source.txt",
			cancellationToken);

		var jobName = $"rakushu-job-log-{jobLog.Id}";

		// Chỉ tìm Job khi transcript thuộc Batch Translation.
		var existingJob =
			await _translateService.FindBatchTranslationJobByNameAsync(
				jobName,
				cancellationToken);

		if (existingJob is null)
		{
			var externalJobId = await CreateTranslationJobAsync(
				transcriptId,
				segmentTexts,
				jobName,
				cancellationToken);

			await _jobLogService.SetJobLabelAsync(
				jobLog.Id,
				string.IsNullOrWhiteSpace(externalJobId) ? jobName : externalJobId,
				cancellationToken);

			return;
		}

		if (jobLog.JobLabel is null)
			await _jobLogService.SetJobLabelAsync(jobLog.Id, existingJob.JobId, cancellationToken);

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

				await _jobLogService.CompleteAsync(
					jobLog.Id,
					JobLogSerializer.Serialize(job),
					cancellationToken);

				return;

			case "FAILED":
			case "STOPPED":

				if (!jobLog.IsFinished)
				{
					await _jobLogService.FailAsync(
						jobLog.Id,
						JobLogSerializer.Serialize(job),
						cancellationToken);
				}

				// Chưa tự retry để tránh tạo Job lặp.
				return;

			default:
				return;
		}
	}

	private async Task<string?> CreateTranslationJobAsync(
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

		return await _translateService.StartBatchTranslationJobAsync(
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