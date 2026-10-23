using MediatR;
using Rakushu.Application.Usecases.Knowledge.KnowledgeMatching.ProcessPendingVideoKnowledgeMatching;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Features.KnowledgeMatching.Handlers;

public sealed class ProcessPendingVideoKnowledgeMatchingHandler
	: IRequestHandler<ProcessPendingVideoKnowledgeMatchingCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IContentKnowledgeCollectionRepository _contentKnowledgeCollectionRepository;
	private readonly IKnowledgeUnitRepository _knowledgeUnitRepository;
	private readonly ITokenRepository _tokenRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly KnowledgeMatchingService _matchingService;
	private readonly LearningUnitOverlapResolver _overlapResolver;

	public ProcessPendingVideoKnowledgeMatchingHandler(
		IVideoRepository videoRepository,
		IContentKnowledgeCollectionRepository contentKnowledgeCollectionRepository,
		IKnowledgeUnitRepository knowledgeUnitRepository,
		ITokenRepository tokenRepository,
		IUnitOfWork unitOfWork,
		KnowledgeMatchingService matchingService,
		LearningUnitOverlapResolver overlapResolver)
	{
		_videoRepository = videoRepository;
		_contentKnowledgeCollectionRepository = contentKnowledgeCollectionRepository;
		_knowledgeUnitRepository = knowledgeUnitRepository;
		_tokenRepository = tokenRepository;
		_unitOfWork = unitOfWork;
		_matchingService = matchingService;
		_overlapResolver = overlapResolver;
	}

	public async Task<Result> Handle(
		ProcessPendingVideoKnowledgeMatchingCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// 1. Lấy tối đa 1 video cần xử lý.
			var videos = await _videoRepository
				.GetProcessingVideosWithReviewedTranscriptAsync(
					1,
					cancellationToken);

			foreach (var video in videos)
			{
				cancellationToken.ThrowIfCancellationRequested();

				var transcript = video.Transcript;

				if (transcript is null ||
					!transcript.IsReviewed ||
					transcript.TranscriptSegments.Any(s => s.LearningUnits.Any()))
				{
					continue;
				}

				// 2. Lấy Knowledge Collection theo Content Category.
				var contentCollections = await _contentKnowledgeCollectionRepository
					.GetByContentCategoryIdOrderedByPriorityAsync(
						video.ContentCategoryId,
						cancellationToken);

				if (contentCollections.Count == 0)
					continue;

				var collectionIds = contentCollections
					.Select(x => x.KnowledgeCollectionId)
					.Distinct()
					.ToList();

				// 3. Lấy Knowledge Unit active thuộc các collection.
				var knowledgeUnits = await _knowledgeUnitRepository
					.GetActiveByCollectionIdsAsync(
						collectionIds,
						cancellationToken);

				if (knowledgeUnits.Count == 0)
					continue;

				// 4. Tạo bảng priority: số nhỏ hơn có độ ưu tiên cao hơn.
				var priorities = contentCollections
					.GroupBy(x => x.KnowledgeCollectionId)
					.ToDictionary(
						group => group.Key,
						group => group.Min(x => x.Priority));

				var knowledgeUnitsById = knowledgeUnits
					.ToDictionary(x => x.Id.Value);

				// 5. Matching từng Transcript Segment độc lập.
				foreach (var segment in transcript.TranscriptSegments
							 .OrderBy(x => x.Sequence))
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tokens = await _tokenRepository
						.GetByTranscriptSegmentIdAsync(
							segment.Id,
							cancellationToken);

					// 6. Tìm tất cả KnowledgeMatchCandidate.
					var candidates = _matchingService.Match(
						knowledgeUnits,
						segment.Id.Value,
						tokens);

					if (candidates.Count == 0)
						continue;

					// 7. Map candidates thành LearningUnit để giữ nguyên
					//    interface Resolve hiện tại.
					var learningUnits = new List<LearningUnit>();

					foreach (var candidate in candidates)
					{
						cancellationToken.ThrowIfCancellationRequested();

						if (!knowledgeUnitsById.TryGetValue(
								candidate.KnowledgeUnitId,
								out var knowledgeUnit))
						{
							continue;
						}

						// Meanings cần được load sẵn trong knowledgeUnits.
						var meaning = knowledgeUnit.Meanings.FirstOrDefault();

						if (meaning is null)
							continue;

						learningUnits.Add(
							LearningUnitMapper.Map(
								candidate,
								meaning,
								1.0));
					}

					if (learningUnits.Count == 0)
						continue;

					// 8. Resolve overlap theo collection priority.
					//    Giữ nguyên hàm Resolve hiện tại.
					var selectedLearningUnits = _overlapResolver.Resolve(
						learningUnits,
						priorities);

					// 9. Chỉ add LearningUnit đã được resolver chọn.
					foreach (var learningUnit in selectedLearningUnits)
					{
						cancellationToken.ThrowIfCancellationRequested();
						segment.AddLearningUnit(learningUnit);
					}
				}
			}

			return Result.Success();
		}, cancellationToken);
	}
}

public static class LearningUnitMapper
{
	public static LearningUnit Map(
		KnowledgeMatchCandidate candidate,
		KnowledgeMeaning knowledgeMeaning,
		double confidence = 1.0)
	{
		return LearningUnit.Create(
			id: LearningUnitId.Create(),
			knowledgeMeaning: knowledgeMeaning,
			transcriptSegmentId: TranscriptSegmentId.From(
				candidate.transcriptSegmentId),
			startTokenIndex: candidate.StartIndex,
			endTokenIndex: candidate.EndIndex,
			confidence: confidence,
			isReviewed: false,
			detectionMethod: MatchingMethod.KnowledgeMatching);
	}
}