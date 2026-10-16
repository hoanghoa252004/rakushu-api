using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.DependencyRelationship.Common;
using Rakushu.Application.Usecases.Knowledge.JapanesePartOfSpeech.Common;
using Rakushu.Domain.Entities.Material.Transcript;

namespace Rakushu.Application.Usecases.Material.Transcript;

public sealed record TranscriptDetailDto(
	Guid Id,
	Guid VideoId,
	string FullText,
	TranscriptSource SourceType,
	string? StorageKey,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	PaginatedList<TranscriptSegmentDetailDto> Segments)
{
	public static TranscriptDetailDto FromEntity(Domain.Entities.Material.Transcript.Transcript e, PaginatedList<TranscriptSegmentDetailDto> segments) =>
		new(
			e.Id.Value,
			e.VideoId.Value,
			e.FullText,
			e.SourceType,
			e.StorageKey,
			e.CreatedAt,
			e.UpdatedAt,
			segments
			);
}

public sealed record TranscriptSegmentDetailDto(
	Guid Id,
	int Sequence,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	List<BunsetsuDetailDto> Bunsetsu)
{
	public static TranscriptSegmentDetailDto FromEntity(Domain.Entities.Material.Transcript.TranscriptSegment.TranscriptSegment e) =>
		new(
			e.Id.Value,
			e.Sequence,
			e.Text,
			e.StartTime,
			e.EndTime,
			e.Bunsetsu
				.OrderBy(b => b.Sequence)
				.Select(BunsetsuDetailDto.FromEntity)
				.ToList());
}

public sealed record BunsetsuDetailDto(
	Guid Id,
	int Sequence,
	string Text,
	int StartIndex,
	int EndIndex,
	List<TokenDetailDto> Tokens)
{
	public static BunsetsuDetailDto FromEntity(Domain.Entities.Material.Bunsetsu.Bunsetsu e) =>
		new(
			e.Id.Value,
			e.Sequence,
			e.Text,
			e.StartIndex,
			e.EndIndex,
			e.Tokens
				.OrderBy(t => t.Sequence)
				.Select(TokenDetailDto.FromEntity)
				.ToList());
}

public sealed record TokenDetailDto(
	Guid Id,
	string Surface,
	string Lemma,
	string Reading,
	JapanesePartOfSpeechDto JapanesePartOfSpeech,
	DependencyRelationshipDto DependencyRelationship,
	int StartIndex,
	int EndIndex,
	int Sequence)
{
	public static TokenDetailDto FromEntity(Domain.Entities.Material.Token.Token e) =>
		new(
			e.Id.Value,
			e.Surface,
			e.Lemma,
			e.Reading,
			new JapanesePartOfSpeechDto()
			{
				Id = e.JapanesePartOfSpeech.Id.Value,
				Code = e.JapanesePartOfSpeech.Code,
				Name = e.JapanesePartOfSpeech.Name,
				JapaneseName= e.JapanesePartOfSpeech.JapaneseName,
				Description = e.JapanesePartOfSpeech.Description
			},
			new DependencyRelationshipDto()
			{
				Id = e.DependencyRelationship.Id.Value,
				Code = e.DependencyRelationship.Code,
				Name = e.DependencyRelationship.Name,
				JapaneseName = e.DependencyRelationship.JapaneseName,
				Description = e.DependencyRelationship.Description
			},
			e.StartIndex,
			e.EndIndex,
			e.Sequence);
}
