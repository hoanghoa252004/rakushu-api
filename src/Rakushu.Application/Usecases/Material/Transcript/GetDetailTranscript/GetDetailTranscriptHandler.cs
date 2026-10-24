using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.DependencyRelationship.Common;
using Rakushu.Application.Usecases.Knowledge.JapanesePartOfSpeech.Common;
using Rakushu.Application.Usecases.Material.Transcript.GetLearningTranscript;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Knowledge.ProficiencyLevel;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptItem;
using Rakushu.Domain.Entities.Material.Video;
using System.Security.AccessControl;

namespace Rakushu.Application.Usecases.Material.Transcript.GetDetailTranscript;

internal sealed class GetDetailTranscriptHandler : IRequestHandler<GetDetailTranscriptQuery, Result<PaginatedList<TranscriptSegmentDetailDto>>>
{
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly IStorageService _storageService;
	public GetDetailTranscriptHandler(ITranscriptRepository transcriptRepository, IStorageService storageService)
	{
		_transcriptRepository = transcriptRepository;
		_storageService = storageService;
	}

	public async Task<Result<PaginatedList<TranscriptSegmentDetailDto>>> Handle(
		GetDetailTranscriptQuery request,
		CancellationToken cancellationToken)
	{
		var transcript = await _transcriptRepository.GetByVideoIdAsync(VideoId.From(request.VideoId), request.PageNumber, request.PageSize, cancellationToken);
		if (transcript is null)
			return Result.Failure<PaginatedList<TranscriptSegmentDetailDto>>(TranscriptErrors.NotFound);

		// Filter segments by startTime if provided
		var filteredSegments = transcript.TranscriptSegments.AsEnumerable();
		if (request.StartTime.HasValue)
		{
			filteredSegments = filteredSegments.Where(s => s.StartTime >= request.StartTime.Value);
		}

		// Order by sequence
		var orderedSegments = filteredSegments.OrderBy(s => s.Sequence).ToList();

		// Calculate total count before paging
		var totalCount = orderedSegments.Count();

		// Apply paging
		var pagedSegments = orderedSegments
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(async x => new TranscriptSegmentDetailDto(
				Id: x.Id.Value,
				Sequence: x.Sequence,
				StartTime: x.StartTime,
				EndTime: x.EndTime,
				Text: x.Text,
				TranscriptSpeaker: new TranscriptSpeakerDetailDto(
					SpeakerLabel: x.TranscriptSpeaker.Label,
					Id: x.TranscriptSpeaker.Speaker?.Id.Value,
					Name: x.TranscriptSpeaker.Speaker?.Name,
					JapaneseName: x.TranscriptSpeaker.Speaker?.JapaneseName,
					ThemeColor: x.TranscriptSpeaker.Speaker?.ThemeColor,
					AvatarKey: x.TranscriptSpeaker.Speaker?.AvatarKey is not null ? await _storageService.CreatePresignedReadUrlAsync(x.TranscriptSpeaker.Speaker.AvatarKey, cancellationToken) : null,
					Description: x.TranscriptSpeaker.Speaker?.Description),
				Subtitle: x.Subtitle is not null ? new SubtitleDetailDto(
					Id: x.Subtitle.Id.Value,
					Sequence: x.Subtitle.Sequence,
					LanguageCode: x.Subtitle.LanguageCode.ToString(),
					Text: x.Subtitle.Text) : null,
				LearningUnits: await Task.WhenAll(x.LearningUnits.Select(async lu => new LearningUnitDetailDto(
					Id: lu.Id.Value,
					KnowledgeUnit: new KnowledgeUnitDto(
						Id: lu.KnowledgeMeaning.KnowledgeUnit.Id.Value,
						KnowledgeType: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeType.ToString(),
						SourceType: lu.KnowledgeMeaning.KnowledgeUnit.SourceType.ToString(),
						ProficiencyLevel: lu.KnowledgeMeaning.KnowledgeUnit.ProficiencyLevel.Code.ToString(),
						Expression: lu.KnowledgeMeaning.KnowledgeUnit.Expression,
						Reading: lu.KnowledgeMeaning.KnowledgeUnit.Reading,
						AudioKey: lu.KnowledgeMeaning.KnowledgeUnit.AudioKey is not null ? await _storageService.CreatePresignedReadUrlAsync(lu.KnowledgeMeaning.KnowledgeUnit.AudioKey, cancellationToken) : null,
						IsActive: lu.KnowledgeMeaning.KnowledgeUnit.IsActive,
						KnowledgeCollection: new KnowledgeCollectionBasicDto(
							Id: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeCollection.Id.Value,
							Scope: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeCollection.Scope.ToString(),
							Type: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeCollection.CollectionType.ToString(),
							Name: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeCollection.Name,
							Description: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeCollection.Description)
						),
					KnowledgeMeaning: new KnowledgeMeaningDto(
						Id: lu.KnowledgeMeaning.Id.Value,
						SortOrder: lu.KnowledgeMeaning.SortOrder,
						Meaning: lu.KnowledgeMeaning.Meaning,
						JapaneseMeaning: lu.KnowledgeMeaning.JapaneseMeaning,
						Description: lu.KnowledgeMeaning.Description,
						AlternativeMeaningCount: lu.KnowledgeMeaning.KnowledgeUnit.Meanings.Count,
						AlternativeMeanings: lu.KnowledgeMeaning.KnowledgeUnit.Meanings
							.Where(m => m.Id != lu.KnowledgeMeaning.Id)
							.Select(m => new KnowledgeMeaningDto(
								Id: m.Id.Value,
								SortOrder: m.SortOrder,
								Meaning: m.Meaning,
								JapaneseMeaning: m.JapaneseMeaning,
								Description: m.Description,
								AlternativeMeaningCount: null,
								AlternativeMeanings: null
							)
						).ToList())
					,
					StartIndex: lu.StartTokenIndex,
					EndIndex: lu.EndTokenIndex,
					DetectionMethod: lu.DetectionMethod.ToString(),
					Confidence: lu.Confidence,
					IsReviewed: lu.IsReviewed
				)).ToList()),
				TranscriptItems: x.TranscriptItems.Select(ti => new TranscriptItemDto(
					Id: ti.Id.Value,
					Sequence: ti.Sequence,
					StartTime: ti.StartTime,
					EndTime: ti.EndTime,
					Type: ti.Type.ToString(),
					Text: ti.Text,
					Confidence: ti.Confidence
				)).ToList(),
				Bunsetsus: x.Bunsetsus.Select(b => new BunsetsuDetailDto(
					Id: b.Id.Value,
					Sequence: b.Sequence,
					Text: b.Text,
					StartIndex: b.StartIndex,
					EndIndex: b.EndIndex,
					Tokens: b.Tokens.Select(t => new TokenDetailDto(
						Id: t.Id.Value,
						Sequence: t.Sequence,
						StartIndex: t.StartIndex,
						EndIndex: t.EndIndex,
						Surface: t.Surface,
						Lemma: t.Lemma,
						Reading: t.Reading,
						JapanesePartOfSpeech: new JapanesePartOfSpeechDto()
						{
							Id = t.JapanesePartOfSpeech.Id.Value,
							Code = t.JapanesePartOfSpeech.Code,
							Name = t.JapanesePartOfSpeech.Name,
							JapaneseName = t.JapanesePartOfSpeech.JapaneseName,
							Description = t.JapanesePartOfSpeech.Description
						},
						DependencyRelationship: new DependencyRelationshipDto()
						{
							Id = t.DependencyRelationship.Id.Value,
							Code = t.DependencyRelationship.Code,
							Name = t.DependencyRelationship.Name,
							JapaneseName = t.DependencyRelationship.JapaneseName,
							Description = t.DependencyRelationship.Description
						}
					)).ToList()
				)).ToList())).ToList();

		var paginatedSegments = PaginatedList<TranscriptSegmentDetailDto>.Create(
			await Task.WhenAll(pagedSegments),
			totalCount,
			request.PageNumber,
			request.PageSize);


		return Result.Success(paginatedSegments);
	}
}
