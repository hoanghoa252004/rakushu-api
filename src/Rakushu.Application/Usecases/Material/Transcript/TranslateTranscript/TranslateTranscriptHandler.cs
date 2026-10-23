using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Infrastructure.Translation;
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

				// Tên job ổn định theo TranscriptId để tìm lại job
				// mà không cần lưu JobId vào database.
				var jobName = $"rakushu-transcript-{transcript.Id}";

				var existingJob =
					await _translateService.FindBatchTranslationJobByNameAsync(
						jobName,
						cancellationToken);

				if (existingJob is null)
				{
					await CreateTranslationJobAsync(
						transcript.Id.ToString(),
						segments.Select(segment => segment.Text).ToList(),
						jobName,
						cancellationToken);

					// Job vừa được tạo chưa có kết quả để lưu.
					continue;
				}

				var job = await _translateService.GetBatchTranslationJobAsync(
					existingJob.JobId,
					cancellationToken);

				switch (job.Status)
				{
					case "SUBMITTED":
					case "IN_PROGRESS":
					case "STOP_REQUESTED":
						// Job chưa hoàn thành, chờ lần chạy handler tiếp theo.
						continue;

					case "COMPLETED":
						await ProcessCompletedTranslationAsync(
							transcript.Id.ToString(),
							segments,
							job,
							cancellationToken);

						continue;

					case "FAILED":
					case "STOPPED":
						// Chưa tự retry để tránh tạo job lặp ngoài ý muốn.
						continue;

					default:
						continue;
				}
			}

			return Result.Success();
		}, cancellationToken);
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

		// Mỗi segment nằm trên một dòng.
		var content = string.Join("\n", segmentTexts);

		var inputPrefix = $"translation/input/{transcriptId}/";
		var inputKey = $"{inputPrefix}source.txt";

		var outputPrefix = $"translation/output/{transcriptId}/";

		// Upload file đầu vào lên S3.
		await _storageService.UploadTextFileAsync(
			inputKey,
			content,
			cancellationToken);

		// Quan trọng: Input URI phải trỏ đến folder, không phải file.
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
		// Output S3 URI là prefix, cần tìm file kết quả thực tế.
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

		var translatedLines = translatedContent
			.Replace("\r\n", "\n")
			.Split('\n')
			.Select(line => line.TrimEnd('\r'))
			.ToList();

		// Chỉ bỏ newline cuối file, không bỏ dòng trống ở giữa.
		if (translatedLines.Count > 0 &&
			translatedLines[^1].Length == 0 &&
			translatedContent.EndsWith('\n'))
		{
			translatedLines.RemoveAt(translatedLines.Count - 1);
		}

		if (translatedLines.Count != segments.Count)
		{
			throw new InvalidOperationException(
				$"Số dòng dịch không khớp số segment. " +
				$"TranscriptId={transcriptId}, " +
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
					$"Bản dịch rỗng tại segment sequence {segment.Sequence}, " +
					$"transcript {transcriptId}.");
			}

			// Không tạo lại subtitle nếu segment đã có subtitle tiếng Việt.
			// Subtitle là navigation property đơn trên TranscriptSegment.
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
					$"Không thể tạo subtitle cho segment sequence {segment.Sequence}, " +
					$"transcript {transcriptId}.");
			}
		}
	}
}