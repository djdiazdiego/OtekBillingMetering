// Ignore spelling: ft³, cubicfoot, cubicfeet, cuft, ccf, hundredcubicfeet, mcf, thousandcubicfeet

using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;

namespace OtekBillingMetering.Execution.Common.UnitConverters;

public static class GasUnitConverter
{
	private const UtilityType Utility = UtilityType.Gas;
	private const AtomicUnitType BaseUnit = AtomicUnitType.CubicFoot;

	private static readonly IReadOnlyDictionary<AtomicUnitType, double> ToBaseFactor =
		new Dictionary<AtomicUnitType, double>
		{
			[AtomicUnitType.CubicFoot] = 1.0,
			[AtomicUnitType.Ccf] = 100.0,
			[AtomicUnitType.Mcf] = 1000.0,
		};

	public static readonly IReadOnlySet<AtomicUnitType> AllowedUnits =
		new HashSet<AtomicUnitType>
		{
			AtomicUnitType.CubicFoot,
			AtomicUnitType.Ccf,
			AtomicUnitType.Mcf
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
			"ccf" or "hundredcubicfeet" or "hundred cubic feet"
				=> AtomicUnitType.Ccf,
			"mcf" or "thousandcubicfeet" or "thousand cubic feet"
				=> AtomicUnitType.Mcf,

			_ => throw new ArgumentException($"Unsupported gas unit code: \"{code}\"")
		};
	}

	public static double ConvertByUnitCode(double value, string fromUnitCode, string toUnitCode)
		=> Convert(value, Parse(fromUnitCode), Parse(toUnitCode));

	private static string Normalize(string? code)
		=> (code ?? string.Empty).Trim().ToLowerInvariant().Replace("_", "").Replace("-", "").Replace(" ", "");
}
