// Ignore spelling: mw

using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;

namespace OtekBillingMetering.Execution.Common.UnitConverters;

public static class ElectricityPowerUnitConverter
{
	private const UtilityType Utility = UtilityType.Electricity;
	private const AtomicUnitType BaseUnit = AtomicUnitType.Watt;

	private static readonly IReadOnlyDictionary<AtomicUnitType, double> ToBaseFactor =
		new Dictionary<AtomicUnitType, double>
		{
			[AtomicUnitType.Watt] = 1.0,
			[AtomicUnitType.Kilowatt] = 1_000.0,
			[AtomicUnitType.Megawatt] = 1_000_000.0,
		};

	public static readonly IReadOnlySet<AtomicUnitType> AllowedUnits =
		new HashSet<AtomicUnitType>
		{
			AtomicUnitType.Watt,
			AtomicUnitType.Kilowatt,
			AtomicUnitType.Megawatt
		};

	public static double Convert(double value, AtomicUnitType from, AtomicUnitType to)
		=> UnitConversion.Convert(value, from, to, BaseUnit, ToBaseFactor, AllowedUnits, Utility);

	public static AtomicUnitType Parse(string code)
	{
		var normalized = Normalize(code);

		return normalized switch
		{
			"w" => AtomicUnitType.Watt,
			"kw" => AtomicUnitType.Kilowatt,
			"mw" => AtomicUnitType.Megawatt,
			_ => throw new ArgumentException($"Unsupported electricity power unit code: \"{code}\"")
		};
	}

	public static double ConvertByUnitCode(double value, string fromUnitCode, string toUnitCode)
		=> Convert(value, Parse(fromUnitCode), Parse(toUnitCode));

	private static string Normalize(string? code)
		=> (code ?? string.Empty).Trim().ToLowerInvariant().Replace("_", "").Replace("-", "").Replace(" ", "");
}
