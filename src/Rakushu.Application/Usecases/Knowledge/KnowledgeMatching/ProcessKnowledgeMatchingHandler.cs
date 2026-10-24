using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMatching;

public sealed class ProcessKnowledgeMatchingHandler: IRequestHandler<ProcessKnowledgeMatchingCommand, Result<IReadOnlyList<KnowledgeMatchCandidate>>>
{
	private readonly IKnowledgeCollectionRepository _knowledgeCollectionRepository;
	private readonly ITokenRepository _tokenRepository;

	private readonly KnowledgeMatchingService _matchingService;

	public ProcessKnowledgeMatchingHandler(
		IKnowledgeCollectionRepository knowledgeCollectionRepository,
		ITokenRepository tokenRepository,
		KnowledgeMatchingService matchingService)
	{
		_knowledgeCollectionRepository = knowledgeCollectionRepository;
		_tokenRepository = tokenRepository;
		_matchingService = matchingService;
	}

	public async Task<Result<IReadOnlyList<KnowledgeMatchCandidate>>> Handle(
		ProcessKnowledgeMatchingCommand request,
		CancellationToken cancellationToken)
	{			
			var collections =await _knowledgeCollectionRepository.GetAllAsync(cancellationToken);

			var tokens = await _tokenRepository.GetByTranscriptSegmentIdAsync(
				TranscriptSegmentId.From(request.TranscriptSegmentId),
				cancellationToken);

		Console.WriteLine("========== TRANSCRIPT TOKENS ==========");

		foreach (var token in tokens)
		{
			Console.WriteLine(
				$"Sequence={token.Sequence}, " +
				$"Surface={token.Surface}, " +
				$"Lemma={token.Lemma}, " +
				$"StartIndex={token.StartIndex}, " +
				$"EndIndex={token.EndIndex}");
		}

		var candidates = _matchingService.Match(
				collections.SelectMany(c => c.KnowledgeUnits).ToList(),
				request.TranscriptSegmentId,
				tokens);

		return Result.Success(candidates);
	}
}