// Ignore spelling: klb, kilopound, kilopounds

using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;

namespace OtekBillingMetering.Execution.Common.UnitConverters;

public static class SteamUnitConverter
{
	private const UtilityType Utility = UtilityType.Steam;
	private const AtomicUnitType BaseUnit = AtomicUnitType.Pound;

	private static readonly IReadOnlyDictionary<AtomicUnitType, double> ToBaseFactor =
		new Dictionary<AtomicUnitType, double>
		{
			[AtomicUnitType.Pound] = 1.0,
			[AtomicUnitType.Kilopound] = 1000.0,
		};

	public static readonly IReadOnlySet<AtomicUnitType> AllowedUnits =
		new HashSet<AtomicUnitType>
		{
			AtomicUnitType.Pound,
			AtomicUnitType.Kilopound
		};

	public static double Convert(double value, AtomicUnitType from, AtomicUnitType to)
		=> UnitConversion.Convert(value, from, to, BaseUnit, ToBaseFactor, AllowedUnits, Utility);

	public static AtomicUnitType Parse(string code)
	{
		var normalized = Normalize(code);

		return normalized switch
		{
			"lb" or "pound" or "pounds"
				=> AtomicUnitType.Pound,
			"klb" or "kilopound" or "kilo-pound" or "kilopounds"
				=> AtomicUnitType.Kilopound,

			_ => throw new ArgumentException($"Unsupported steam unit code: \"{code}\"")
		};
	}

	public static double ConvertByUnitCode(double value, string fromUnitCode, string toUnitCode)
		=> Convert(value, Parse(fromUnitCode), Parse(toUnitCode));

	private static string Normalize(string? code)
		=> (code ?? string.Empty).Trim().ToLowerInvariant().Replace("_", "").Replace("-", "").Replace(" ", "");
}
