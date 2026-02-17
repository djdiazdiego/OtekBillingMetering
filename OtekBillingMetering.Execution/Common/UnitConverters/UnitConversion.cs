using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;

namespace OtekBillingMetering.Execution.Common.UnitConverters;

public static class UnitConversion
{
	public static double Convert(
		double value,
		AtomicUnitType from,
		AtomicUnitType to,
		AtomicUnitType baseUnit,
		IReadOnlyDictionary<AtomicUnitType, double> toBaseFactor,
		IReadOnlySet<AtomicUnitType> allowedUnits,
		UtilityType utility)
	{
		if(!double.IsFinite(value))
		{
			throw new ArgumentOutOfRangeException(nameof(value), value, "value must be finite.");
		}

		if(from == to)
		{
			return value;
		}

		if(!allowedUnits.Contains(from))
		{
			throw new ArgumentOutOfRangeException(nameof(from), from,
				$"Unit '{from}' is not allowed for utility '{utility}'.");
		}

		if(!allowedUnits.Contains(to))
		{
			throw new ArgumentOutOfRangeException(nameof(to), to,
				$"Unit '{to}' is not allowed for utility '{utility}'.");
		}

		if(!toBaseFactor.TryGetValue(from, out var fromFactor))
		{
			throw new InvalidOperationException(
				$"Missing conversion factor to base unit '{baseUnit}' for '{from}' in '{utility}'.");
		}

		if(!toBaseFactor.TryGetValue(to, out var toFactor))
		{
			throw new InvalidOperationException(
				$"Missing conversion factor to base unit '{baseUnit}' for '{to}' in '{utility}'.");
		}

		var inBase = value * fromFactor;
		return inBase / toFactor;
	}
}
