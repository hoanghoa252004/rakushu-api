using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Token.GetTokens;

internal sealed class GetTokensHandler : IRequestHandler<GetTokensQuery, Result<IReadOnlyCollection<TokenDto>>>
{
	private readonly IVideoRepository _videoRepository;

	public GetTokensHandler(IVideoRepository videoRepository)
	{
		_videoRepository = videoRepository;
	}

	public async Task<Result<IReadOnlyCollection<TokenDto>>> Handle(GetTokensQuery request, CancellationToken cancellationToken)
	{
		var videos = await _videoRepository.GetAllAsync(cancellationToken);
		var tokens = videos
			.Where(v => v.Transcript != null)
			.SelectMany(v => v.Transcript!.TranscriptSegments)
			.SelectMany(s => s.Bunsetsu)
			.SelectMany(b => b.Tokens)
			.Select(TokenDto.FromEntity)
			.ToArray();

		return Result.Success<IReadOnlyCollection<TokenDto>>(tokens);
	}
}