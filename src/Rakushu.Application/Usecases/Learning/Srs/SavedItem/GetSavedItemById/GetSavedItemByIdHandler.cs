using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSavedItemById;

internal sealed class GetSavedItemByIdHandler : IRequestHandler<GetSavedItemByIdQuery, Result<SavedItemDetailDto>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IStorageService _storageService;
	private readonly ICurrentUserContext _currentUserContext;

	public GetSavedItemByIdHandler(
		ISavedItemRepository savedItemRepository,
		ILearningUnitRepository learningUnitRepository,
		IStorageService storageService,
		ICurrentUserContext currentUserContext)
	{
		_savedItemRepository = savedItemRepository;
		_learningUnitRepository = learningUnitRepository;
		_storageService = storageService;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<SavedItemDetailDto>> Handle(
		GetSavedItemByIdQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<SavedItemDetailDto>(UserErrors.NotFound);

		var itemId = SavedItemId.From(request.Id);
		var item = await _savedItemRepository.GetByIdAsync(itemId, cancellationToken);
		if (item is null || item.UserId != userId)
			return Result.Failure<SavedItemDetailDto>(SavedItemErrors.NotFound);

		var mediaContexts = await _learningUnitRepository.GetMediaContextsByMeaningIdsAsync(
			new[] { item.KnowledgeMeaningId.Value },
			maxVideosPerMeaning: 10,
			cancellationToken);

		var videoContextDtos = new List<SrsItemVideoContextDto>();
		if (mediaContexts.TryGetValue(item.KnowledgeMeaningId.Value, out var mediaList))
		{
			foreach (var media in mediaList)
			{
				string? videoUrl = null;
				if (!string.IsNullOrWhiteSpace(media.StorageKey))
				{
					try
					{
						videoUrl = await _storageService.CreatePresignedReadUrlAsync(media.StorageKey, cancellationToken);
					}
					catch
					{
						videoUrl = media.SourceUrl;
					}
				}
				else if (!string.IsNullOrWhiteSpace(media.SourceUrl))
				{
					videoUrl = media.SourceUrl;
				}

				videoContextDtos.Add(new SrsItemVideoContextDto(
					media.VideoId,
					media.VideoTitle,
					videoUrl,
					media.StartTime,
					media.EndTime,
					media.OriginalSentence,
					media.TranslatedSentence,
					media.StartTokenIndex,
					media.EndTokenIndex,
					media.TranscriptSegmentId,
					media.LearningUnitId));
			}
		}

		return Result.Success(new SavedItemDetailDto(
			item.Id.Value,
			item.KnowledgeMeaningId.Value,
			item.Expression,
			item.Reading,
			item.Meaning,
			item.IsActive,
			item.SavedAt,
			videoContextDtos,
			item.IsRevoked));
	}
}
