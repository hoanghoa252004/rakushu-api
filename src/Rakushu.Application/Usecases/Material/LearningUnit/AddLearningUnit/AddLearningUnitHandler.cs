using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.LearningUnit.AddLearningUnit;

internal sealed class AddLearningUnitHandler : IRequestHandler<AddLearningUnitCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IKnowledgeMeaningRepository _meaningRepository;
	private readonly IUnitOfWork _unitOfWork;

	public AddLearningUnitHandler(
		IVideoRepository videoRepository,
		IKnowledgeMeaningRepository meaningRepository,
		IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_meaningRepository = meaningRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(AddLearningUnitCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var video = await _videoRepository.GetByIdAsync(VideoId.From(request.VideoId), cancellationToken);
			if (video is null)
				return Result.Failure<Guid>(VideoErrors.NotFound);

			if (video.Transcript is null)
				return Result.Failure<Guid>(TranscriptErrors.NotFound);

			var segmentId = TranscriptSegmentId.From(request.SegmentId);
			var segment = video.Transcript.TranscriptSegments.FirstOrDefault(s => s.Id == segmentId);
			if (segment is null)
				return Result.Failure<Guid>(TranscriptSegmentErrors.NotFound);

			var meaning = await _meaningRepository.GetByIdAsync(
				KnowledgeMeaningId.From(request.KnowledgeMeaningId), cancellationToken);
			if (meaning is null || meaning.KnowledgeUnit is null || !meaning.KnowledgeUnit.IsActive)
				return Result.Failure<Guid>(LearningUnitErrors.KnowledgeMeaningNotFound);

			var tokenCount = segment.Bunsetsus.Sum(b => b.Tokens.Count);
			if (request.EndTokenIndex >= tokenCount)
				return Result.Failure<Guid>(LearningUnitErrors.InvalidTokenRange);

			if (segment.LearningUnits.Any(x =>
				x.KnowledgeMeaningId == meaning.Id &&
				x.StartTokenIndex == request.StartTokenIndex &&
				x.EndTokenIndex == request.EndTokenIndex))
				return Result.Failure<Guid>(LearningUnitErrors.AlreadyExists);

			var learningUnit = Domain.Entities.Material.LearningUnit.LearningUnit.Create(
				LearningUnitId.Create(),
				meaning,
				segment.Id,
				request.StartTokenIndex,
				request.EndTokenIndex,
				1.0,
				true,
				MatchingMethod.CuratorDefine);

			var addResult = segment.AddLearningUnit(learningUnit);
			if (addResult.IsFailure)
				return Result.Failure<Guid>(addResult.Error);

			return Result.Success(learningUnit.Id.Value);
		}, cancellationToken);
	}
}
