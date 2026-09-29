using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using System;

namespace Rakushu.Domain.Entities.DictionaryEntry;

public sealed class DictionaryEntry : AggregateRoot<DictionaryEntryId>
{
	public string Term { get; private set; } = null!;
	public string Reading { get; private set; } = null!;
	public string Pos { get; private set; } = null!;
	public string Meaning { get; private set; } = null!;
	public string? MeaningVietnamese { get; private set; }
	public string? JlptLevel { get; private set; }
	public string? WordType { get; private set; }
	public string? AudioUrl { get; private set; }
	public string? DefinitionTags { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	private DictionaryEntry() { }

	private DictionaryEntry(
		DictionaryEntryId id,
		string term,
		string reading,
		string pos,
		string meaning,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? meaningVietnamese = null,
		string? jlptLevel = null,
		string? wordType = null,
		string? audioUrl = null,
		string? definitionTags = null) : base(id)
	{
		Term = term;
		Reading = reading;
		Pos = pos;
		Meaning = meaning;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		MeaningVietnamese = meaningVietnamese;
		JlptLevel = jlptLevel;
		WordType = wordType;
		AudioUrl = audioUrl;
		DefinitionTags = definitionTags;
	}

	public static Result<DictionaryEntry> Create(
		string term,
		string reading,
		string pos,
		string meaning,
		string? meaningVietnamese = null,
		string? jlptLevel = null,
		string? wordType = null,
		string? audioUrl = null,
		string? definitionTags = null,
		DateTimeOffset? createdAt = null)
	{
		var now = createdAt ?? DateTimeOffset.UtcNow;
		return Result<DictionaryEntry>.Success(new DictionaryEntry(
			DictionaryEntryId.Create(),
			term.Trim(),
			reading.Trim(),
			pos.Trim(),
			meaning.Trim(),
			now,
			now,
			meaningVietnamese?.Trim(),
			jlptLevel?.Trim(),
			wordType?.Trim(),
			audioUrl?.Trim(),
			definitionTags?.Trim()
		));
	}

	public void Update(
		string reading,
		string pos,
		string meaning,
		string? meaningVietnamese = null,
		string? jlptLevel = null,
		DateTimeOffset? updatedAt = null)
	{
		Reading = reading.Trim();
		Pos = pos.Trim();
		Meaning = meaning.Trim();
		MeaningVietnamese = meaningVietnamese?.Trim() ?? MeaningVietnamese;
		JlptLevel = jlptLevel?.Trim() ?? JlptLevel;
		UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
	}
}
