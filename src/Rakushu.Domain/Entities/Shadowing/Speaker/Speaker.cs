using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSpeaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.Speaker;

public sealed class Speaker : Entity<SpeakerId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string JapaneseName { get; private set; } = null!;
	public string? AvatarKey { get; private set; }
	public string? Description { get; private set; }
	public string ThemeColor { get; private set; } = null!;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	private readonly List<TranscriptSpeaker> _transcriptSpeakers = new();
	public IReadOnlyCollection<TranscriptSpeaker> TranscriptSpeakers => _transcriptSpeakers.AsReadOnly();


	private Speaker() { }

	private Speaker(
		SpeakerId id,
		string code,
		string name,
		string japaneseName,
		string? avatarKey,
		string? description,
		string themeColor,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
		: base(id)
	{
		Name = name;
		Code = code;
		JapaneseName = japaneseName;
		AvatarKey = avatarKey;
		Description = description;
		ThemeColor = themeColor;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<Speaker> Create(
		string code,
		string name,
		string japaneseName,
		string? avatarKey,
		string? description,
		string themeColor,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
			return Result.Failure<Speaker>(SpeakerErrors.InvalidName);

		if (description?.Length > 1000)
			return Result.Failure<Speaker>(SpeakerErrors.InvalidDescription);

		if (!IsValidHexColor(themeColor))
			return Result.Failure<Speaker>(SpeakerErrors.InvalidThemeColor);

		return Result.Success(new Speaker(
			SpeakerId.Create(),
			code,
			name.Trim(),
			japaneseName,
			avatarKey,
			description?.Trim(),
			themeColor,
			createdAt,
			updatedAt));
	}

	public Result Update(
		string name,
		string japaneseName,
		string? avatarKey,
		string? description,
		string themeColor,
		DateTimeOffset updatedAt)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
			return Result.Failure(SpeakerErrors.InvalidName);

		if (description?.Length > 1000)
			return Result.Failure(SpeakerErrors.InvalidDescription);

		if (!IsValidHexColor(themeColor))
			return Result.Failure(SpeakerErrors.InvalidThemeColor);

		Name = name.Trim();
		JapaneseName = japaneseName;
		AvatarKey = avatarKey;
		Description = description?.Trim();
		ThemeColor = themeColor;
		UpdatedAt = updatedAt;
		return Result.Success();
	}

	private static bool IsValidHexColor(string? color)
	{
		if (string.IsNullOrWhiteSpace(color) || color.Length != 7)
			return false;

		if (color[0] != '#')
			return false;

		return color.AsSpan(1).IndexOfAnyExcept("0123456789ABCDEFabcdef") < 0;
	}
}