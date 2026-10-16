using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Token.CreateToken;

internal sealed class CreateTokenHandler : IRequestHandler<CreateTokenCommand, Result<Guid>>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTokenHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var bunsetsuId = BunsetsuId.From(request.bunsetsuId);
			var video = await _videoRepository.GetByBunsetsuIdAsync(bunsetsuId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure<Guid>(BunsetsuErrors.NotFound);

			var bunsetsu = video.Transcript.TranscriptSegments
				.SelectMany(s => s.Bunsetsu)
				.FirstOrDefault(b => b.Id == bunsetsuId);

			if (bunsetsu is null)
				return Result.Failure<Guid>(BunsetsuErrors.NotFound);

			var result = bunsetsu.AddToken(
				request.surface,
				request.lemma,
				request.reading,
				JapanesePartOfSpeechId.From(request.japanesePartOfSpeechId),
				DependencyRelationshipId.From(request.dependencyRelationshipId),
				request.startIndex,
				request.endIndex,
				request.sequence);

			if (result.IsFailure)
				return Result.Failure<Guid>(result.Error);

			return Result.Success(result.Value.Id.Value);
		}, cancellationToken);
	}
}