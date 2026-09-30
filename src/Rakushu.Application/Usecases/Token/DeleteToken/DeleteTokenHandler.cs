using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Token.DeleteToken;

internal sealed class DeleteTokenHandler : IRequestHandler<DeleteTokenCommand, Result>
{
	private readonly IVideoRepository _videoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTokenHandler(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
	{
		_videoRepository = videoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteTokenCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var tokenId = TokenId.From(request.TokenId);
			var video = await _videoRepository.GetByTokenIdAsync(tokenId, cancellationToken);
			if (video is null || video.Transcript is null)
				return Result.Failure(TokenErrors.NotFound);

			var bunsetsu = video.Transcript.TranscriptSegments
				.SelectMany(s => s.Bunsetsu)
				.FirstOrDefault(b => b.Tokens.Any(t => t.Id == tokenId));

			if (bunsetsu is null)
				return Result.Failure(TokenErrors.NotFound);

			return bunsetsu.RemoveToken(tokenId);
		}, cancellationToken);
	}
}