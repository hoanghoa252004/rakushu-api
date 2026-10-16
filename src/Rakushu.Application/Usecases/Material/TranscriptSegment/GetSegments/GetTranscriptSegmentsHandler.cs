using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.GetSegments;

internal sealed class GetTranscriptSegmentsHandler : IRequestHandler<GetTranscriptSegmentsQuery, Result<PaginatedList<TranscriptSegmentDetailDto>>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTranscriptSegmentsHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<PaginatedList<TranscriptSegmentDetailDto>>> Handle(
		GetTranscriptSegmentsQuery request,
		CancellationToken cancellationToken)
	{
		var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<PaginatedList<TranscriptSegmentDetailDto>>(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure<PaginatedList<TranscriptSegmentDetailDto>>(
				TranscriptErrors.NotFound);

		// Filter segments by startTime if provided
		var filteredSegments = video.Transcript.TranscriptSegments.AsEnumerable();
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
			.Select(TranscriptSegmentDetailDto.FromEntity)
			.ToList();

		var paginatedSegments = PaginatedList<TranscriptSegmentDetailDto>.Create(
			pagedSegments,
			totalCount,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedSegments);
	}
}
