using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Transcript.GetLearningTranscript;


internal sealed class GetLearningTranscriptHandler : IRequestHandler<GetLearningTranscriptQuery, Result<PaginatedList<TranscriptSegmentBasicDto>>>
{
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly IStorageService _storageService;
	public GetLearningTranscriptHandler(ITranscriptRepository transcriptRepository, IStorageService storageService)
	{
		_transcriptRepository = transcriptRepository;
		_storageService = storageService;
	}

	public async Task<Result<PaginatedList<TranscriptSegmentBasicDto>>> Handle(
		GetLearningTranscriptQuery request,
		CancellationToken cancellationToken)
	{
		var transcript = await _transcriptRepository.GetByVideoIdAsync(VideoId.From(request.VideoId), request.PageNumber, request.PageSize, cancellationToken);
		if (transcript is null)
			return Result.Failure<PaginatedList<TranscriptSegmentBasicDto>>(TranscriptErrors.NotFound);

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
			.Select(async x => new TranscriptSegmentBasicDto(
				Id: x.Id.Value,
				Sequence: x.Sequence,
				StartTime: x.StartTime,
				EndTime: x.EndTime,
				Speaker: new SpeakerBasicDto(
					SpeakerLabel: x.TranscriptSpeaker.Label,
					Id: x.TranscriptSpeaker.Speaker?.Id.Value,
					Name: x.TranscriptSpeaker.Speaker?.Name,
					JapaneseName: x.TranscriptSpeaker.Speaker?.JapaneseName,
					ThemeColor: x.TranscriptSpeaker.Speaker?.ThemeColor,
					AvatarKey: x.TranscriptSpeaker.Speaker?.AvatarKey is not null ? await _storageService.CreatePresignedReadUrlAsync(x.TranscriptSpeaker.Speaker.AvatarKey, cancellationToken) : null,
					Description: x.TranscriptSpeaker.Speaker?.Description),
				Text: x.Text,
				Subtitle: new SubtitleBasicDto(
					Id: x.Subtitle.Id.Value,
					Sequence: x.Subtitle.Sequence,
					LanguageCode: x.Subtitle.LanguageCode.ToString(),
					Text: x.Subtitle.Text),
				LearningUnits: x.LearningUnits.Select(lu => new LearningUnitBasicDto(
					Id: lu.Id.Value,
					StartIndex: lu.StartTokenIndex,
					EndIndex: lu.EndTokenIndex,
					Collection: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeCollection.Name,
					Type: lu.KnowledgeMeaning.KnowledgeUnit.KnowledgeType.ToString(),
					Expression: lu.KnowledgeMeaning.KnowledgeUnit.Expression,
					Meaning: lu.KnowledgeMeaning.Meaning
				)).ToList()))
			.ToList();

		var paginatedSegments = PaginatedList<TranscriptSegmentBasicDto>.Create(
			await Task.WhenAll(pagedSegments),
			totalCount,
			request.PageNumber,
			request.PageSize);


		return Result.Success(paginatedSegments);
	}
}
