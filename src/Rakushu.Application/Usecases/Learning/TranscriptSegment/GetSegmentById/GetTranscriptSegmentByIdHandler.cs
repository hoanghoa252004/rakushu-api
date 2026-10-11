using MediatR;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.GetSegmentById;

internal sealed class GetTranscriptSegmentByIdHandler : IRequestHandler<GetTranscriptSegmentByIdQuery, Result<TranscriptSegmentDetailDto>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTranscriptSegmentByIdHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<TranscriptSegmentDetailDto>> Handle(
		GetTranscriptSegmentByIdQuery request,
		CancellationToken cancellationToken)
	{
		var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
		if (video is null)
			return Result.Failure<TranscriptSegmentDetailDto>(VideoErrors.NotFound);

		if (video.Transcript is null)
			return Result.Failure<TranscriptSegmentDetailDto>(
				Domain.Entities.Video.Transcript.TranscriptErrors.NotFound);

		// Find segment by ID
		var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == TranscriptSegmentId.From(request.SegmentId));
		if (segment is null)
			return Result.Failure<TranscriptSegmentDetailDto>(
				TranscriptSegmentErrors.NotFound);

		return Result.Success(TranscriptSegmentDetailDto.FromEntity(segment));
	}
}
