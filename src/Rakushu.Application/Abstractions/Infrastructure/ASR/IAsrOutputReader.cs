using Rakushu.Application.Abstractions.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.ASR;

public interface IAsrOutputReader
{
	// Outputs
	IAsyncEnumerable<TranscribeItemAsrReadDto> ReadItemsAsync(string storageKey, CancellationToken cancellationToken);
	Task<string> ReadFullTextAsync(string storageKey, CancellationToken cancellationToken);
	Task<IReadOnlyList<TranscribeSpeakerAsrReadDto>> ReadSpeakersAsync(string storageKey, CancellationToken cancellationToken);
}

// 2. Output
public sealed record TranscribeItemAsrReadDto(
	int Sequence,
	string Text,
	string Type,
	TimeSpan? StartTime,
	TimeSpan? EndTime,
	decimal? Confidence,
	string? SpeakerLabel);

public sealed record TranscribeSpeakerAsrReadDto(
	string Label);

public sealed record TranscribeSegmentAsrReadDto(
	int Sequence,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string? SpeakerLabel);

public sealed record TranscribeOutputDto(
	string FullText,
	IReadOnlyList<TranscribeSpeakerAsrReadDto> Speakers,
	IReadOnlyList<TranscribeItemAsrReadDto> Items);

