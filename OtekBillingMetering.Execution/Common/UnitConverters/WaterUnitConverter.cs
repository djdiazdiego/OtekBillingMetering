// Ignore Spelling: ft³, cubicfoot, cubicfeet, cuft, usgal, ccf, hundredcubicfeet,
// Ignore Spelling: milliongallons, millionusgallons, acft, acrefoot

using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;

namespace OtekBillingMetering.Execution.Common.UnitConverters;

public static class WaterUnitConverter
{
	private const UtilityType Utility = UtilityType.Water;
	private const AtomicUnitType BaseUnit = AtomicUnitType.CubicFoot;

	private static readonly IReadOnlyDictionary<AtomicUnitType, double> ToBaseFactor =
		new Dictionary<AtomicUnitType, double>
		{
			[AtomicUnitType.CubicFoot] = 1.0,
			[AtomicUnitType.UsGallon] = 0.133680556,
			[AtomicUnitType.Ccf] = 100.0,
			[AtomicUnitType.MillionUsGallons] = 133_680.556,
			[AtomicUnitType.AcreFoot] = 43_560.0,
		};

	public static readonly IReadOnlySet<AtomicUnitType> AllowedUnits =
		new HashSet<AtomicUnitType>
		{
			AtomicUnitType.CubicFoot,
			AtomicUnitType.UsGallon,
			AtomicUnitType.Ccf,
			AtomicUnitType.MillionUsGallons,
			AtomicUnitType.AcreFoot
		};

	public static double Convert(double value, AtomicUnitType from, AtomicUnitType to)
		=> UnitConversion.Convert(value, from, to, BaseUnit, ToBaseFactor, AllowedUnits, Utility);

	public static AtomicUnitType Parse(string code)
	{
		var normalized = Normalize(code);

		return normalized switch
		{
			"ft3" or "ft^3" or "ft³" or "cubicfoot" or "cubicfeet" or "cuft" or "cu ft" or "cf"
				=> AtomicUnitType.CubicFoot,
			"usgal" or "us gal" or "gallon" or "gal" or "gallons"
				=> AtomicUnitType.UsGallon,
			"ccf" or "hundredcubicfeet" or "hundred cubic feet"
				=> AtomicUnitType.Ccf,
			"mg" or "milliongallons" or "million gallons" or "millionusgallons"
				=> AtomicUnitType.MillionUsGallons,
			"ac-ft" or "acft" or "acrefoot" or "acre-foot" or "acre feet"
				=> AtomicUnitType.AcreFoot,

			_ => throw new ArgumentException($"Unsupported water unit code: \"{code}\"")
		};
	}

	public static double ConvertByUnitCode(double value, string fromUnitCode, string toUnitCode)
		=> Convert(value, Parse(fromUnitCode), Parse(toUnitCode));

	private static string Normalize(string? code)
		=> (code ?? string.Empty).Trim().ToLowerInvariant().Replace("_", "").Replace("-", "").Replace(" ", "");
}
