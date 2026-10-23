using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.ASR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Material.Transcript.ImportTranscript;

public sealed class ImportGeneratedTranscriptHandler
	: IRequestHandler<ImportGeneratedTranscriptCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IAsrOutputReader _asrOutputReader;
	private readonly IStorageService _storageService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;
	private readonly IAsrService _asrService;
	private readonly TranscriptSegmentBuilder _transcriptSegmentBuilder;

	public ImportGeneratedTranscriptHandler(
		IStorageService storageService,
		IVideoRepository videoRepository,
		IAsrOutputReader asrOutputReader,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		IAsrService asrService,
		TranscriptSegmentBuilder transcriptSegmentBuilder)
	{
		_storageService = storageService;
		_videoRepository = videoRepository;
		_asrOutputReader = asrOutputReader;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
		_asrService = asrService;
		_transcriptSegmentBuilder = transcriptSegmentBuilder;
	}

	public async Task<Result> Handle(
		ImportGeneratedTranscriptCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var videos = (await _videoRepository.GetAllAsync(cancellationToken))
				.Where(video =>
					video.Status == VideoStatus.Processing &&
					video.Transcript == null)
				.ToList();

			foreach (var video in videos)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var videoAsset = video.MediaAssets
					.FirstOrDefault(asset =>
						asset.AssetType == MediaAssetType.Video);

				if (videoAsset is null)
					continue;

				// 1. Generate a stable AWS Transcribe job name.
				var jobName = GetTranscriptionJobName(video.Id.Value);

				// 3. Check whether the corresponding job already exists.
				var job = await _asrService.GetJobAsync(
					jobName,
					cancellationToken);

				// 4. If no job exists, start one and process it later.
				if (job is null)
				{
					await _asrService.StartAsync(
						new StartTranscriptionRequest
						(
							jobName,
							_storageService.GetS3MediaUri(videoAsset.StorageKey),
							"mp4",
							"ja-JP",
							GetTranscriptStorageKey(jobName)
						),
						cancellationToken);

					continue;
				}

				// 5. Import the transcript only when the job has completed.
				if (job.Status != "COMPLETED")
				{
					// IN_PROGRESS: wait for the next worker execution.
					// FAILED: log the failure reason and handle retries separately.
					continue;
				}

				// 6. Read the completed transcription output from S3.
				var fullText = await _asrOutputReader.ReadFullTextAsync(
					GetTranscriptStorageKey(jobName),
					cancellationToken);

				var speakers = await _asrOutputReader.ReadSpeakersAsync(
					GetTranscriptStorageKey(jobName),
					cancellationToken);

				var items = new List<TranscribeItemAsrReadDto>();

				await foreach (var item in _asrOutputReader.ReadItemsAsync(
					GetTranscriptStorageKey(jobName),
					cancellationToken))
				{
					items.Add(item);
				}

				// 7. Create the transcript entity.
				var now = _systemClock.UtcNow;

				var transcriptResult =
					Rakushu.Domain.Entities.Material.Transcript.Transcript.Create(
						video.Id,
						fullText,
						now,
						now);

				if (transcriptResult.IsFailure)
				{
					return Result.Failure(transcriptResult.Error);
				}

				var transcript = transcriptResult.Value;

				transcript.SetStorageKey(GetTranscriptStorageKey(jobName));

				// 8. Create speakers and map their IDs.
				var speakerIds =
					new Dictionary<string, TranscriptSpeakerId>(
						StringComparer.Ordinal);

				foreach (var speaker in speakers)
				{
					var speakerResult = transcript.AddSpeaker(speaker.Label);

					if (speakerResult.IsFailure)
					{
						return Result.Failure(speakerResult.Error);
					}

					speakerIds[speaker.Label] = speakerResult.Value.Id;
				}

				// 9. Build segment drafts.
				var segmentDrafts = _transcriptSegmentBuilder.Build(
					items.Select(item => new TranscribeItemBuildDto(
						item.Sequence,
						item.Text,
						item.Type,
						item.StartTime,
						item.EndTime,
						item.Confidence,
						item.SpeakerLabel)));

				foreach (var draft in segmentDrafts)
				{
					TranscriptSpeakerId speakerId = null!;

					if (!string.IsNullOrWhiteSpace(draft.SpeakerLabel) &&
						speakerIds.TryGetValue(
							draft.SpeakerLabel,
							out var matchedSpeakerId))
					{
						speakerId = matchedSpeakerId;
					}

					var segmentResult = transcript.AddSegment(
						speakerId,
						draft.Text,
						draft.StartTime,
						draft.EndTime,
						draft.Sequence,
						now);

					if (segmentResult.IsFailure)
					{
						return Result.Failure(segmentResult.Error);
					}

					var segment = segmentResult.Value;

					// 10. Create transcript items.
					for (var i = 0; i < draft.Items.Count; i++)
					{
						var item = draft.Items[i];

						var itemType = item.Type switch
						{
							"pronunciation" =>
								TranscriptItemType.Pronunciation,

							"punctuation" =>
								TranscriptItemType.Punctuation,

							_ => throw new InvalidDataException(
								$"Unsupported AWS Transcribe item type: '{item.Type}'.")
						};

						// Punctuation items might not have timestamps.
						var startTime = item.StartTime ?? draft.EndTime;
						var endTime = item.EndTime ?? startTime;

						var itemResult = segment.AddItem(
							item.Text,
							itemType,
							startTime,
							endTime,
							item.Confidence ?? 0m,
							i + 1);

						if (itemResult.IsFailure)
						{
							return Result.Failure(itemResult.Error);
						}
					}
				}

				// 11. Attach the completed transcript to the video.
				video.SetTranscript(transcript);
			}

			return Result.Success();

		}, cancellationToken);
	}

	private static string GetTranscriptionJobName(Guid videoId)
	{
		return $"asr-video_id-{videoId:D}";
	}

	private static string GetTranscriptStorageKey(string jobName)
	{
		return $"transcripts/{jobName}.json";
	}
}