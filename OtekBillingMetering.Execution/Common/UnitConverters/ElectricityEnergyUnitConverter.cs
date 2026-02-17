// Ignore spelling: wh, kwh, mwh, gwh

using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;

namespace OtekBillingMetering.Execution.Common.UnitConverters;

public static class ElectricityEnergyUnitConverter
{
	private const UtilityType Utility = UtilityType.Electricity;
	private const AtomicUnitType BaseUnit = AtomicUnitType.WattHour;

	private static readonly IReadOnlyDictionary<AtomicUnitType, double> ToBaseFactor =
		new Dictionary<AtomicUnitType, double>
		{
			[AtomicUnitType.WattHour] = 1.0,
			[AtomicUnitType.KilowattHour] = 1_000.0,
			[AtomicUnitType.MegawattHour] = 1_000_000.0,
			[AtomicUnitType.GigawattHour] = 1_000_000_000.0,
		};

	public static readonly IReadOnlySet<AtomicUnitType> AllowedUnits =
		new HashSet<AtomicUnitType>
		{
			AtomicUnitType.WattHour,
			AtomicUnitType.KilowattHour,
			AtomicUnitType.MegawattHour,
			AtomicUnitType.GigawattHour
		};

	public static double Convert(double value, AtomicUnitType from, AtomicUnitType to)
		=> UnitConversion.Convert(value, from, to, BaseUnit, ToBaseFactor, AllowedUnits, Utility);

	public static AtomicUnitType Parse(string code)
	{
		var normalized = Normalize(code);

		return normalized switch
		{
			"wh" => AtomicUnitType.WattHour,
			"kwh" => AtomicUnitType.KilowattHour,
			"mwh" => AtomicUnitType.MegawattHour,
			"gwh" => AtomicUnitType.GigawattHour,
			_ => throw new ArgumentException($"Unsupported electricity energy unit code: \"{code}\"")
		};
	}

	public static double ConvertByUnitCode(double value, string fromUnitCode, string toUnitCode)
		=> Convert(value, Parse(fromUnitCode), Parse(toUnitCode));

	private static string Normalize(string? code)
		=> (code ?? string.Empty).Trim().ToLowerInvariant().Replace("_", "").Replace("-", "").Replace(" ", "");
}
