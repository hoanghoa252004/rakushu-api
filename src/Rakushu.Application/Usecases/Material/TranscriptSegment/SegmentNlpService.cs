using Rakushu.Application.Abstractions.Infrastructure.Nlp;
using Rakushu.Application.Usecases.Material.Nlp.NlpProcess;
using Rakushu.Domain.Entities.Knowledge.DependencyRelationship;
using Rakushu.Domain.Entities.Knowledge.JapanesePartOfSpeech;
using Rakushu.Domain.Entities.Material.Bunsetsu;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment;

public interface ISegmentNlpService
{
	// Re-runs NLP for the segment and replaces all its old bunsetsus and tokens.
	Task ReplaceAsync(
		Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment segment,
		CancellationToken cancellationToken);
}

internal sealed class SegmentNlpService : ISegmentNlpService
{
	private readonly INlpService _nlpService;
	private readonly IJapanesePartOfSpeechRepository _partOfSpeechRepository;
	private readonly IDependencyRelationshipRepository _dependencyRepository;

	public SegmentNlpService(
		INlpService nlpService,
		IJapanesePartOfSpeechRepository partOfSpeechRepository,
		IDependencyRelationshipRepository dependencyRepository)
	{
		_nlpService = nlpService;
		_partOfSpeechRepository = partOfSpeechRepository;
		_dependencyRepository = dependencyRepository;
	}

	public async Task ReplaceAsync(
		Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment segment,
		CancellationToken cancellationToken)
	{
		var posByCode = (await _partOfSpeechRepository.GetAllAsync(cancellationToken))
			.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);
		var depByCode = (await _dependencyRepository.GetAllAsync(cancellationToken))
			.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

		var nlpRequest = new List<TranscriptSegmentNlpRequest>
		{
			new()
			{
				Id = segment.Id.Value,
				Sequence = segment.Sequence,
				Text = segment.Text,
				StartTime = segment.StartTime.ToString(),
				EndTime = segment.EndTime.ToString(),
				TranscriptId = segment.TranscriptId.Value,
				TranscriptSpeakerId = segment.TranscriptSpeakerId?.Value
			}
		};

		var response = await _nlpService.TranscriptAnalyzeAsync(nlpRequest, cancellationToken);
		var segmentResult = response.Results.FirstOrDefault(x => x.Id == segment.Id.Value)
			?? throw new InvalidOperationException($"NLP response missing segment {segment.Id}.");

		// Remove old bunsetsus (tokens are removed with them)
		segment.ClearBunsetsus();

		var bunsetsuByTokenIndex = new Dictionary<int, Bunsetsu>();

		foreach (var b in segmentResult.Bunsetsu.OrderBy(x => x.StartToken))
		{
			var created = Bunsetsu.Create(segment.Id, b.Text, b.StartIndex, b.EndIndex, b.StartToken);
			if (created.IsFailure)
				throw new InvalidOperationException($"Failed to create Bunsetsu for segment {segment.Id}: {created.Error}");

			var bunsetsu = created.Value;
			for (var i = b.StartToken; i < b.EndToken; i++)
			{
				if (!bunsetsuByTokenIndex.TryAdd(i, bunsetsu))
					throw new InvalidOperationException($"Token index {i} belongs to multiple Bunsetsu in segment {segment.Id}.");
			}

			segment.AddBunsetsu(bunsetsu);
		}

		foreach (var t in segmentResult.Tokens.OrderBy(x => x.Index))
		{
			if (!bunsetsuByTokenIndex.TryGetValue(t.Index, out var bunsetsu))
				throw new InvalidOperationException($"Cannot find Bunsetsu for token index {t.Index} in segment {segment.Id}.");

			if (!posByCode.TryGetValue(t.Tag, out var pos))
				throw new InvalidOperationException($"JapanesePartOfSpeech code '{t.Tag}' was not found.");

			if (!depByCode.TryGetValue(t.Dependency, out var dep))
				throw new InvalidOperationException($"DependencyRelationship code '{t.Dependency}' was not found.");

			var tokenResult = bunsetsu.AddToken(
				surface: t.Text,
				lemma: t.Lemma,
				reading: t.Reading,
				headSequence: t.Head,
				japanesePartOfSpeechId: pos.Id,
				dependencyRelationshipId: dep.Id,
				startIndex: t.StartIndex,
				endIndex: t.StartIndex + t.Text.Length,
				sequence: t.Index);

			if (tokenResult.IsFailure)
				throw new InvalidOperationException($"Failed to create Token '{t.Text}' in segment {segment.Id}: {tokenResult.Error}");
		}
	}
}
