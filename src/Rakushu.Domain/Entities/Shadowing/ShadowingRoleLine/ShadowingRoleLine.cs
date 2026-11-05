using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

public sealed class ShadowingRoleLine : Entity<ShadowingRoleLineId>
{
	public ShadowingMaterialId ShadowingMaterialId { get; private set; } = null!;
	public ShadowingRoleId ShadowingRoleId { get; private set; } = null!;
	public int SortOrder { get; private set; }
	public string Text { get; private set; } = null!;
	public TimeSpan StartTime { get; private set; }
	public TimeSpan EndTime { get; private set; }
	public string AudioKey { get; private set; } = null!;

	// COMPUTED PROPERTIES
	public TimeSpan Duration => EndTime - StartTime;


	// NAVIGATION PROPERTIES
	// ShadowingMaterial
	public ShadowingMaterial.ShadowingMaterial ShadowingMaterial { get; private set; } = null!;

	// ShadowingRole
	public ShadowingRole.ShadowingRole ShadowingRole { get; private set; } = null!;

	// ShadowingRoleLineTranscriptSegments
	//private readonly List<ShadowingRoleLineTranscriptSegment>
	//	_transcriptSegments = new();

	//public IReadOnlyCollection<ShadowingRoleLineTranscriptSegment>
	//	TranscriptSegments => _transcriptSegments.AsReadOnly();

	private ShadowingRoleLine() { }

	private ShadowingRoleLine(
		ShadowingRoleLineId id,
		ShadowingMaterialId shadowingMaterialId,
		ShadowingRoleId shadowingRoleId,
		int sortOrder,
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		string audioKey) : base(id)
	{
		ShadowingMaterialId = shadowingMaterialId;
		ShadowingRoleId = shadowingRoleId;
		SortOrder = sortOrder;
		Text = text;
		StartTime = startTime;
		EndTime = endTime;
		AudioKey = audioKey;
	}

	public static Result<ShadowingRoleLine> Create(
		ShadowingMaterialId shadowingMaterialId,
		ShadowingRoleId shadowingRoleId,
		int sortOrder,
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		string audioKey)
	{
		var validation = Validate(sortOrder, text, startTime, endTime, audioKey);
		if (validation.IsFailure)
			return Result.Failure<ShadowingRoleLine>(validation.Error);

		return Result.Success(new ShadowingRoleLine(
			ShadowingRoleLineId.Create(),
			shadowingMaterialId,
			shadowingRoleId,
			sortOrder,
			text.Trim(),
			startTime,
			endTime,
			audioKey.Trim()));
	}

	public Result Update(
		ShadowingRoleId shadowingRoleId,
		int sortOrder,
		string text,
		TimeSpan startTime,
		TimeSpan endTime,
		string audioKey)
	{
		var validation = Validate(sortOrder, text, startTime, endTime, audioKey);
		if (validation.IsFailure)
			return validation;

		ShadowingRoleId = shadowingRoleId;
		SortOrder = sortOrder;
		Text = text.Trim();
		StartTime = startTime;
		EndTime = endTime;
		AudioKey = audioKey.Trim();
		return Result.Success();
	}

	private static Result Validate(int sortOrder, string text, TimeSpan startTime, TimeSpan endTime, string audioKey)
	{
		if (sortOrder < 1)
			return Result.Failure(ShadowingRoleLineErrors.InvalidSortOrder);

		if (string.IsNullOrWhiteSpace(text))
			return Result.Failure(ShadowingRoleLineErrors.InvalidText);

		if (startTime < TimeSpan.Zero || endTime <= startTime)
			return Result.Failure(ShadowingRoleLineErrors.InvalidTimeRange);

		if (string.IsNullOrWhiteSpace(audioKey) || audioKey.Trim().Length > 500)
			return Result.Failure(ShadowingRoleLineErrors.InvalidAudioKey);

		return Result.Success();
	}
}
