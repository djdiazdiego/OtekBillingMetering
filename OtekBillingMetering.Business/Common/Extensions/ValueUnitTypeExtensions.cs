using OtekBillingMetering.Business.Common.Types.Units;

namespace OtekBillingMetering.Business.Common.Extensions;

public static class ValueUnitTypeExtensions
{
	public static ValueUnitCategory Category(this ValueUnitType unit) => unit switch
	{
		ValueUnitType.None => ValueUnitCategory.None,
		ValueUnitType.Usd => ValueUnitCategory.Money,
		ValueUnitType.UsdPerDay => ValueUnitCategory.Usage,
		ValueUnitType.Percent => ValueUnitCategory.Percentage,
		_ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
	};

	public static ValueUnitType[] GetUnitTypesFromCategory(this ValueUnitCategory category) => category switch
	{
		ValueUnitCategory.None => [ValueUnitType.None],
		ValueUnitCategory.Money => [ValueUnitType.Usd],
		ValueUnitCategory.Usage => [ValueUnitType.UsdPerDay],
		ValueUnitCategory.Percentage => [ValueUnitType.Percent],
		_ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
	};

	public static string Symbol(this ValueUnitType unit) => unit switch
	{
		ValueUnitType.None => ValueUnitType.None.GetEnumMemberValue(),
		ValueUnitType.Usd => ValueUnitType.Usd.GetEnumMemberValue(),
		ValueUnitType.UsdPerDay => ValueUnitType.UsdPerDay.GetEnumMemberValue(),
		ValueUnitType.Percent => ValueUnitType.Percent.GetEnumMemberValue(),
		_ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
	};
}
