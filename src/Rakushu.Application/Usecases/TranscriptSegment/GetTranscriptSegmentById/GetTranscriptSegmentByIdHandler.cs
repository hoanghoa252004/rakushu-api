using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.TranscriptSegment.GetTranscriptSegmentById;

internal sealed class GetTranscriptSegmentByIdHandler : IRequestHandler<GetTranscriptSegmentByIdQuery, Result<TranscriptSegmentDto>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTranscriptSegmentByIdHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<TranscriptSegmentDto>> Handle(GetTranscriptSegmentByIdQuery request, CancellationToken cancellationToken)
	{
		var segmentId = TranscriptSegmentId.From(request.TranscriptSegmentId);
		var video = await _videoRepository.GetByTranscriptSegmentIdAsync(segmentId, cancellationToken);
		if (video is null || video.Transcript is null)
			return Result.Failure<TranscriptSegmentDto>(TranscriptSegmentErrors.NotFound);

		var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
		if (segment is null)
			return Result.Failure<TranscriptSegmentDto>(TranscriptSegmentErrors.NotFound);

		return Result.Success(TranscriptSegmentDto.FromEntity(segment));
	}
}