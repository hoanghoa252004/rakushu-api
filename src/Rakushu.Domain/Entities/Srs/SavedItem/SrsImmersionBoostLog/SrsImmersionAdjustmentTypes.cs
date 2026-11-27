namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog;

public static class SrsImmersionAdjustmentTypes
{
	public const string OverdueRescue = "OverdueRescue";
	public const string DueTodayBonus = "DueTodayBonus";

	public static bool IsValid(string? value)
	{
		return value is OverdueRescue or DueTodayBonus;
	}
}
