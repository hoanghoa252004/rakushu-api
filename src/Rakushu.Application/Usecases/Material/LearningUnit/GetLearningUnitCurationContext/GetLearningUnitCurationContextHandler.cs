using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video.MediaAsset;

namespace Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitCurationContext;

internal sealed class GetLearningUnitCurationContextHandler : IRequestHandler<GetLearningUnitCurationContextQuery, Result<LearningUnitCurationContextDto>>
{
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IStorageService _storageService;

	public GetLearningUnitCurationContextHandler(
		ILearningUnitRepository learningUnitRepository,
		IStorageService storageService)
	{
		_learningUnitRepository = learningUnitRepository;
		_storageService = storageService;
	}

	public async Task<Result<LearningUnitCurationContextDto>> Handle(
		GetLearningUnitCurationContextQuery request,
		CancellationToken cancellationToken)
	{
		var (unit, segmentUnits) = await _learningUnitRepository.GetCurationContextByIdAsync(
			LearningUnitId.From(request.LearningUnitId),
			cancellationToken);

		if (unit is null)
			return Result.Failure<LearningUnitCurationContextDto>(LearningUnitErrors.NotFound);

		var segment = unit.TranscriptSegment;
		var video = segment?.Transcript?.Video;

		if (segment is null || video is null)
			return Result.Failure<LearningUnitCurationContextDto>(LearningUnitErrors.NotFound);

		// Resolve playable video URL
		var videoAsset = video.MediaAssets
			.FirstOrDefault(a => a.AssetType == MediaAssetType.Video && !string.IsNullOrEmpty(a.StorageKey));

		string? videoUrl = null;
		if (videoAsset?.StorageKey is not null)
		{
			videoUrl = await _storageService.CreatePresignedReadUrlAsync(videoAsset.StorageKey, cancellationToken);
		}
		else
		{
			videoUrl = video.SourceUrl;
		}

		// Flatten tokens from Bunsetsus
		var tokens = segment.Bunsetsus
			.SelectMany(b => b.Tokens)
			.OrderBy(t => t.Sequence)
			.Select(t => new CurationTokenDto(
				t.Id.Value,
				t.Sequence,
				t.StartIndex,
				t.EndIndex,
				t.Surface,
				t.Lemma,
				t.Reading,
				t.JapanesePartOfSpeech?.Name))
			.ToList();

		// Segment's learning units
		var segmentLearningUnits = segmentUnits
			.OrderBy(lu => lu.StartTokenIndex)
			.Select(lu => new SegmentLearningUnitDto(
				lu.Id.Value,
				lu.KnowledgeMeaningId.Value,
				lu.KnowledgeMeaning.KnowledgeUnit.Expression,
				lu.KnowledgeMeaning.Meaning,
				lu.StartTokenIndex,
				lu.EndTokenIndex,
				lu.Id == unit.Id))
			.ToList();

		SubtitleDetailDto? subtitleDto = null;
		if (segment.Subtitle is not null)
		{
			subtitleDto = new SubtitleDetailDto(
				segment.Subtitle.Id.Value,
				segment.Subtitle.Sequence,
				segment.Subtitle.LanguageCode.ToString(),
				segment.Subtitle.Text);
		}

		var dto = new LearningUnitCurationContextDto(
			LearningUnitId: unit.Id.Value,
			VideoId: video.Id.Value,
			VideoTitle: video.Title,
			VideoUrl: videoUrl,
			SegmentId: segment.Id.Value,
			SegmentSequence: segment.Sequence,
			StartTime: segment.StartTime,
			EndTime: segment.EndTime,
			OriginalSentence: segment.Text,
			Subtitle: subtitleDto,
			StartTokenIndex: unit.StartTokenIndex,
			EndTokenIndex: unit.EndTokenIndex,
			KnowledgeMeaningId: unit.KnowledgeMeaningId.Value,
			Expression: unit.KnowledgeMeaning.KnowledgeUnit.Expression,
			Reading: unit.KnowledgeMeaning.KnowledgeUnit.Reading,
			Meaning: unit.KnowledgeMeaning.Meaning,
			Tokens: tokens,
			SegmentLearningUnits: segmentLearningUnits
		);

		return Result.Success(dto);
	}
}
