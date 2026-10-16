using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.ASR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rakushu.Application.Usecases.Material.Transcript.ImportTranscript;

public sealed class ImportTranscriptHandler : IRequestHandler<ImportTranscriptCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IAsrOutputReader _asrOutputReader;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;


	private readonly TranscriptSegmentBuilder _transcriptSegmentBuilder;

	public ImportTranscriptHandler(
		IVideoRepository videoRepository,
		IAsrOutputReader asrOutputReader,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock,
		TranscriptSegmentBuilder transcriptSegmentBuilder)
	{
		_videoRepository = videoRepository;
		_asrOutputReader = asrOutputReader;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
		_transcriptSegmentBuilder = transcriptSegmentBuilder;
	}

	public async Task<Result> Handle(
		ImportTranscriptCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync( async () =>
		{
			var videos = (await _videoRepository.GetAllAsync(cancellationToken))
					.Where(video => video.Status == VideoStatus.Processing
								 && video.Transcript == null);

			foreach (var video in videos)
			{

				var videoAsset = video.MediaAssets
					.FirstOrDefault(asset =>
						asset.AssetType == MediaAssetType.Video);

				if (videoAsset is null)
					continue;

				var transcriptStorageKey =
					GetTranscriptStorageKey(videoAsset.StorageKey);

				var fullText = await _asrOutputReader
					.ReadFullTextAsync(
						transcriptStorageKey,
						cancellationToken);

				var speakers = await _asrOutputReader
					.ReadSpeakersAsync(
						transcriptStorageKey,
						cancellationToken);

				var items = new List<TranscribeItemAsrReadDto>();

				await foreach (var item in _asrOutputReader
					.ReadItemsAsync(
						transcriptStorageKey,
						cancellationToken))
				{
					items.Add(item);
				}

				// 1. Create a new transcript entity
				var transcriptResult = Rakushu.Domain.Entities.Material.Transcript.Transcript.Create(
					video.Id,
					fullText,
					DateTimeOffset.UtcNow,
					DateTimeOffset.UtcNow);

				if (transcriptResult.IsFailure)
				{
					return Result.Failure(transcriptResult.Error);
				}

				var transcript = transcriptResult.Value;

				transcript.SetStorageKey(transcriptStorageKey);

				// 2. Create speakers and map their IDs
				var speakerIds = new Dictionary<string, TranscriptSpeakerId>(StringComparer.Ordinal);

				foreach (var speaker in speakers)
				{
					var result = transcript.AddSpeaker(speaker.Label);

					if (result.IsFailure)
					{
						// Xử lý lỗi.
						return Result.Failure(result.Error);
					}

					speakerIds[speaker.Label] = result.Value.Id;
				}

				// 3. Create segments and associate them with the correct speaker IDs

				var segmentDrafts = _transcriptSegmentBuilder.Build(items.Select(item => new TranscribeItemBuildDto(
					item.Sequence,
					item.Text,
					item.Type,
					item.StartTime,
					item.EndTime,
					item.Sequence,
					item.SpeakerLabel
					)));

				var now = _systemClock.UtcNow;

				foreach (var draft in segmentDrafts)
				{
					TranscriptSpeakerId speakerId = null!;

					if (!string.IsNullOrWhiteSpace(draft.SpeakerLabel) &&
						speakerIds.TryGetValue(draft.SpeakerLabel, out var id))
					{
						speakerId = id;
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

					for (var i = 0; i < draft.Items.Count; i++)
					{
						var item = draft.Items[i];

						// Ánh xạ Type sang TranscriptItemType theo enum thực tế.
						var itemType = item.Type switch
						{
							"pronunciation" => TranscriptItemType.Pronunciation,

							"punctuation" => TranscriptItemType.Punctuation,

							_ => throw new InvalidDataException(
								$"Unsupported AWS Transcribe item type: '{item.Type}'.")
						};

						// AWS punctuation items thường không có timestamp.
						// Dùng timestamp của token trước cho dấu câu.
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

				video.SetTranscript(transcript);
			}

			return Result.Success();
		}, cancellationToken);
	}

	private static string GetTranscriptStorageKey(
		string videoStorageKey)
	{
		var fileName = videoStorageKey
			.Split('/')
			.Last();

		var fileNameWithoutExtension =
			Path.GetFileNameWithoutExtension(fileName);

		return $"transcripts/{fileNameWithoutExtension}.json";
	}
}