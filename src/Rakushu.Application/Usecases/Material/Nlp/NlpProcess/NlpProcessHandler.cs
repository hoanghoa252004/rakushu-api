using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.Nlp;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Nlp.NlpProcess;

public sealed class NlpProcessHandler : IRequestHandler<NlpProcessCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IJapanesePartOfSpeechRepository _partOfSpeechRepository;
	private readonly IDependencyRelationshipRepository _dependencyRepository;
	private readonly INlpService _nlpService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public NlpProcessHandler(
		IVideoRepository videoRepository,
		IJapanesePartOfSpeechRepository partOfSpeechRepository,
		IDependencyRelationshipRepository dependencyRepository,
		INlpService nlpService,
		IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_videoRepository = videoRepository;
		_partOfSpeechRepository = partOfSpeechRepository;
		_dependencyRepository = dependencyRepository;
		_nlpService = nlpService;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(
		NlpProcessCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Lấy video:
			var videos = (await _videoRepository.GetAllAsync(cancellationToken))
				.Where(v => v.Status == VideoStatus.Processing
							&& v.Transcript != null
							&& v.Transcript.IsReviewed == true
							&& v.Transcript.TranscriptSegments.All(s => s.Bunsetsus.Any() == false))
				.ToList();

			if (videos.Count == 0)
				return Result.Success();

			// 2. Nạp danh mục một lần.
			var partOfSpeeches = await _partOfSpeechRepository.GetAllAsync(cancellationToken);

			var dependencies = await _dependencyRepository.GetAllAsync(cancellationToken);

			var partOfSpeechByCode = partOfSpeeches.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

			var dependencyByCode = dependencies.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

			// 3. Xử lý từng video.
			foreach (var video in videos)
			{
				//cancellationToken.ThrowIfCancellationRequested();

				var transcript = video.Transcript;

				var segments = transcript.TranscriptSegments
					.OrderBy(x => x.Sequence)
					.ToList();

				if (segments.Count == 0)
					continue;

				// 4. Map segment thành request DTO.
				var nlpRequest = segments
					.Select(segment => new TranscriptSegmentNlpRequest
					{
						Id = segment.Id.Value,
						Sequence = segment.Sequence,
						Text = segment.Text,

						// Điều chỉnh theo cách lưu thời gian của domain.
						StartTime = segment.StartTime.ToString(),
						EndTime = segment.EndTime.ToString(),

						TranscriptId = transcript.Id.Value,

						TranscriptSpeakerId =
							segment.TranscriptSpeakerId == null
								? null
								: segment.TranscriptSpeakerId.Value
					})
					.ToList();

				// 5. Gọi GiNZA.
				var nlpResponse = await _nlpService.TranscriptAnalyzeAsync(
					nlpRequest,
					cancellationToken);

				// 6. Index kết quả theo TranscriptSegmentId.
				var resultsBySegmentId = nlpResponse.Results
					.ToDictionary(x => x.Id);

				var now = _systemClock.UtcNow;

				// 7. Xử lý từng segment.
				foreach (var segment in segments)
				{
					if (!resultsBySegmentId.TryGetValue(
							segment.Id.Value,
							out var segmentResult))
					{
						throw new InvalidOperationException(
							$"NLP response missing segment {segment.Id}.");
					}

					// 7.1. Tạo Bunsetsu trước.
					// Key: chỉ số token của GiNZA.
					// Value: Bunsetsu tương ứng.
					var bunsetsuByTokenIndex = new Dictionary<int, Rakushu.Domain.Entities.Material.Bunsetsu.Bunsetsu>();

					foreach (var bunsetsuResult in segmentResult.Bunsetsu
								 .OrderBy(x => x.StartToken))
					{
						var createBunsetsuResult = Rakushu.Domain.Entities.Material.Bunsetsu.Bunsetsu.Create(
							segment.Id,
							bunsetsuResult.Text,
							bunsetsuResult.StartIndex,
							bunsetsuResult.EndIndex,
							bunsetsuResult.StartToken);

						if (createBunsetsuResult.IsFailure)
							throw new InvalidOperationException(
								$"Failed to create Bunsetsu for segment {segment.Id}: " +
								$"{createBunsetsuResult.Error}");

						var bunsetsu = createBunsetsuResult.Value;

						// Đăng ký Bunsetsu với TranscriptSegment/repository tại đây.
						// Cần dùng đúng aggregate method hoặc repository của dự án.

						// 7.2. Ánh xạ các token thuộc Bunsetsu.
						// end_token là exclusive.
						for (
							var tokenIndex = bunsetsuResult.StartToken;
							tokenIndex < bunsetsuResult.EndToken;
							tokenIndex++)
						{
							if (!bunsetsuByTokenIndex.TryAdd(
									tokenIndex,
									bunsetsu))
							{
								throw new InvalidOperationException(
									$"Token index {tokenIndex} belongs to multiple Bunsetsu " +
									$"in segment {segment.Id}.");
							}
						}

						segment.AddBunsetsu(bunsetsu);
					}

					// 7.3–7.5. Map danh mục và tạo Token.
					foreach (var tokenResult in segmentResult.Tokens
								 .OrderBy(x => x.Index))
					{
						if (!bunsetsuByTokenIndex.TryGetValue(
								tokenResult.Index,
								out var bunsetsu))
						{
							throw new InvalidOperationException(
								$"Cannot find Bunsetsu for token index {tokenResult.Index} " +
								$"in segment {segment.Id}.");
						}

						// Map chính xác JapanesePartOfSpeech bằng tag.
						if (!partOfSpeechByCode.TryGetValue(
								tokenResult.Tag,
								out var partOfSpeech))
						{
							throw new InvalidOperationException(
								$"JapanesePartOfSpeech code '{tokenResult.Tag}' " +
								$"was not found for segment {segment.Id}, " +
								$"token '{tokenResult.Text}'.");
						}

						// Map chính xác DependencyRelationship bằng dep.
						if (!dependencyByCode.TryGetValue(
								tokenResult.Dependency,
								out var dependency))
						{
							throw new InvalidOperationException(
								$"DependencyRelationship code '{tokenResult.Dependency}' " +
								$"was not found for segment {segment.Id}, " +
								$"token '{tokenResult.Text}'.");
						}

						var tokenStartIndex = tokenResult.StartIndex;
						var tokenEndIndex = tokenStartIndex + tokenResult.Text.Length;

						// Lưu ý: Index của token và bunsetsu phải cùng hệ tọa độ.
						// Sequence hiện giữ nguyên chỉ số i từ GiNZA (bắt đầu từ 0).
						var createTokenResult = bunsetsu.AddToken(
							surface: tokenResult.Text,
							lemma: tokenResult.Lemma,
							reading: tokenResult.Reading,
							headSequence: tokenResult.Head,
							japanesePartOfSpeechId: partOfSpeech.Id,
							dependencyRelationshipId: dependency.Id,
							startIndex: tokenStartIndex,
							endIndex: tokenEndIndex,
							sequence: tokenResult.Index);

						if (createTokenResult.IsFailure)
						{
							throw new InvalidOperationException(
								$"Failed to create Token '{tokenResult.Text}' " +
								$"in segment {segment.Id}: {createTokenResult.Error}");
						}
					}
				}
			}

			return Result.Success();
		}, cancellationToken);
	}
}