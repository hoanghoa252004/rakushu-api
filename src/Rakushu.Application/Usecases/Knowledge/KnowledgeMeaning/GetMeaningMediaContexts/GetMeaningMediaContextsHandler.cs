using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.LearningUnit;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.GetMeaningMediaContexts;

internal sealed class GetMeaningMediaContextsHandler : IRequestHandler<GetMeaningMediaContextsQuery, Result<IReadOnlyList<MeaningMediaContextDto>>>
{
	private readonly IKnowledgeMeaningRepository _knowledgeMeaningRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IStorageService _storageService;

	public GetMeaningMediaContextsHandler(
		IKnowledgeMeaningRepository knowledgeMeaningRepository,
		ILearningUnitRepository learningUnitRepository,
		IStorageService storageService)
	{
		_knowledgeMeaningRepository = knowledgeMeaningRepository;
		_learningUnitRepository = learningUnitRepository;
		_storageService = storageService;
	}

	public async Task<Result<IReadOnlyList<MeaningMediaContextDto>>> Handle(
		GetMeaningMediaContextsQuery request,
		CancellationToken cancellationToken)
	{
		var meaningId = KnowledgeMeaningId.From(request.MeaningId);
		var meaning = await _knowledgeMeaningRepository.GetByIdAsync(meaningId, cancellationToken);
		if (meaning is null)
		{
			return Result.Failure<IReadOnlyList<MeaningMediaContextDto>>(KnowledgeMeaningErrors.NotFound);
		}

		int maxCount = Math.Clamp(request.MaxCount, 1, 50);
		var mediaContexts = await _learningUnitRepository.GetMediaContextsByMeaningIdAsync(
			meaningId,
			maxCount,
			cancellationToken);

		if (mediaContexts.Count == 0)
		{
			return Result.Success<IReadOnlyList<MeaningMediaContextDto>>([]);
		}

		var expression = meaning.KnowledgeUnit?.Expression?.Trim();
		var urlCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		var result = new List<MeaningMediaContextDto>(mediaContexts.Count);

		foreach (var mc in mediaContexts)
		{
			string? videoUrl = null;
			if (!string.IsNullOrWhiteSpace(mc.StorageKey))
			{
				if (urlCache.TryGetValue(mc.StorageKey, out var cached))
				{
					videoUrl = cached;
				}
				else
				{
					try
					{
						videoUrl = await _storageService.CreatePresignedReadUrlAsync(mc.StorageKey, cancellationToken);
						urlCache[mc.StorageKey] = videoUrl;
					}
					catch
					{
						videoUrl = mc.SourceUrl;
					}
				}
			}
			else if (!string.IsNullOrWhiteSpace(mc.SourceUrl))
			{
				videoUrl = mc.SourceUrl;
			}

			string clozeSentence = mc.OriginalSentence;
			if (!string.IsNullOrEmpty(expression) && mc.OriginalSentence.Contains(expression, StringComparison.Ordinal))
			{
				int index = mc.OriginalSentence.IndexOf(expression, StringComparison.Ordinal);
				clozeSentence = string.Concat(
					mc.OriginalSentence.AsSpan(0, index),
					" ( ? ) ",
					mc.OriginalSentence.AsSpan(index + expression.Length));
			}

			result.Add(new MeaningMediaContextDto(
				LearningUnitId: mc.LearningUnitId ?? Guid.Empty,
				TranscriptSegmentId: mc.TranscriptSegmentId ?? Guid.Empty,
				VideoId: mc.VideoId,
				VideoTitle: mc.VideoTitle,
				VideoUrl: videoUrl,
				StorageKey: mc.StorageKey,
				StartTime: mc.StartTime.TotalSeconds,
				EndTime: mc.EndTime.TotalSeconds,
				OriginalSentence: mc.OriginalSentence,
				ClozeSentence: clozeSentence,
				TranslatedSentence: mc.TranslatedSentence,
				StartTokenIndex: mc.StartTokenIndex,
				EndTokenIndex: mc.EndTokenIndex));
		}

		return Result.Success<IReadOnlyList<MeaningMediaContextDto>>(result);
	}
}
