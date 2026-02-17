using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Common.Types.DateTime;
using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.RateModels.Types;

namespace OtekBillingMetering.Business.ValueObjects.RateTiers;

public sealed record RateTierDefinition
{
	public string Name { get; init; } = null!;
	public RateTierType RateTierType { get; init; }
	public double Multiplier { get; init; }
	public double? UnitsPerCharge { get; init; }
	public AtomicUnitType UnitType { get; init; }
	public double? From { get; init; }
	public double? To { get; init; }

	public IReadOnlyList<string>? PercentageTargetTierNames { get; init; }

	public MonthType? MonthFrom { get; init; }
	public MonthType? MonthTo { get; init; }
	public int? DayOfMonthFrom { get; init; }
	public int? DayOfMonthTo { get; init; }
	public WeekdayType? WeekdayFrom { get; init; }
	public WeekdayType? WeekdayTo { get; init; }
	public TimeOnly? TimeOfDayFrom { get; init; }
	public TimeOnly? TimeOfDayTo { get; init; }

	private RateTierDefinition() { }

	public static RateTierDefinition Create(
		string name,
		RateTierType rateTierType,
		double multiplier,
		double? unitsPerCharge,
		AtomicUnitType unitType,
		double? from,
		double? to,
		MonthType? monthFrom,
		MonthType? monthTo,
		int? dayOfMonthFrom,
		int? dayOfMonthTo,
		WeekdayType? weekdayFrom,
		WeekdayType? weekdayTo,
		TimeOnly? timeOfDayFrom,
		TimeOnly? timeOfDayTo,
		IReadOnlyList<string>? percentageTargetTierNames)
	{
		if(string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("RateTier name is required.");
		}

		if(multiplier <= 0)
		{
			throw new DomainValidationException("Multiplier must be > 0.");
		}

		if(unitsPerCharge.HasValue && unitsPerCharge.Value < 0)
		{
			throw new DomainValidationException("UnitsPerCharge cannot be negative.");
		}

		var requiresRange = rateTierType is RateTierType.RangeUsage or RateTierType.TimeRelatedRangeUsage;
		if(requiresRange)
		{
			if(!from.HasValue)
			{
				throw new DomainValidationException("From is required for ranged tiers.");
			}

			if(to.HasValue && from.Value >= to.Value)
			{
				throw new DomainValidationException("From must be < To when To is provided.");
			}
		}
		else
		{
			from = null;
			to = null;
		}

		if(rateTierType is RateTierType.TimeRelatedPercentage || rateTierType is RateTierType.Percentage)
		{
			if(percentageTargetTierNames is null || percentageTargetTierNames.Count == 0)
			{
				throw new DomainValidationException("PercentageTargetTierNames is required for percentage tiers.");
			}

			if(percentageTargetTierNames.Any(string.IsNullOrWhiteSpace))
			{
				throw new DomainValidationException("PercentageTargetTierNames cannot contain null or whitespace values.");
			}

			percentageTargetTierNames = [.. percentageTargetTierNames
				.Distinct(StringComparer.Ordinal)];
		}

		var requiresTimeWindow = rateTierType is
			RateTierType.TimeRelatedFlat or
			RateTierType.TimeRelatedRangeUsage or
			RateTierType.TimeRelatedFlatUsage or
			RateTierType.TimeRelatedPercentage;

		if(!requiresTimeWindow)
		{
			monthFrom = monthTo = null;
			dayOfMonthFrom = dayOfMonthTo = null;
			weekdayFrom = weekdayTo = null;
			timeOfDayFrom = timeOfDayTo = null;
		}
		else
		{
			EnsurePairParity(monthFrom, monthTo, "MonthFrom", "MonthTo");
			EnsurePairParity(dayOfMonthFrom, dayOfMonthTo, "DayOfMonthFrom", "DayOfMonthTo");

			if(dayOfMonthFrom.HasValue)
			{
				EnsureDayOfMonthRange(dayOfMonthFrom.Value, "DayOfMonthFrom");
			}

			if(dayOfMonthTo.HasValue)
			{
				EnsureDayOfMonthRange(dayOfMonthTo.Value, "DayOfMonthTo");
			}

			if(monthFrom.HasValue && monthTo.HasValue && dayOfMonthFrom.HasValue && dayOfMonthTo.HasValue)
			{
				if(monthFrom.Value == monthTo.Value && dayOfMonthFrom.Value > dayOfMonthTo.Value)
				{
					throw new DomainValidationException(
						"DayOfMonthFrom must be <= DayOfMonthTo when MonthFrom == MonthTo.");
				}
			}

			if(weekdayFrom.HasValue && weekdayTo.HasValue)
			{
				if(weekdayFrom.Value == weekdayTo.Value)
				{
					throw new DomainValidationException("WeekdayFrom must be different than WeekdayTo when both are provided.");
				}
			}

			if(weekdayFrom.HasValue && !weekdayTo.HasValue)
			{
				throw new DomainValidationException("WeekdayTo must be provided if WeekdayFrom is provided.");
			}

			if(timeOfDayFrom.HasValue && timeOfDayTo.HasValue && timeOfDayFrom.Value >= timeOfDayTo.Value)
			{
				throw new DomainValidationException("TimeOfDayFrom must be < TimeOfDayTo.");
			}

			if(!timeOfDayFrom.HasValue && timeOfDayTo.HasValue)
			{
				throw new DomainValidationException("TimeOfDayFrom must be provided if TimeOfDayTo is provided.");
			}

			var isTimeRelated =
				monthFrom.HasValue || dayOfMonthFrom.HasValue || weekdayFrom.HasValue || timeOfDayFrom.HasValue;

			if(!isTimeRelated)
			{
				throw new DomainValidationException("This tier type requires time constraints.");
			}
		}

		return new RateTierDefinition
		{
			Name = name.Trim(),
			RateTierType = rateTierType,
			Multiplier = multiplier,
			UnitsPerCharge = unitsPerCharge,
			UnitType = unitType,
			From = from,
			To = to,
			MonthFrom = monthFrom,
			MonthTo = monthTo,
			DayOfMonthFrom = dayOfMonthFrom,
			DayOfMonthTo = dayOfMonthTo,
			WeekdayFrom = weekdayFrom,
			WeekdayTo = weekdayTo,
			TimeOfDayFrom = timeOfDayFrom,
			TimeOfDayTo = timeOfDayTo
		};
	}

	private static void EnsurePairParity<T>(T? from, T? to, string fromName, string toName) where T : struct
	{
		if(from.HasValue ^ to.HasValue)
		{
			throw new DomainValidationException($"{fromName} and {toName} must be provided together.");
		}
	}

	private static void EnsureDayOfMonthRange(int value, string fieldName)
	{
		if(value is < 1 or > 31)
		{
			throw new DomainValidationException($"{fieldName} must be between 1 and 31.");
		}
	}
}
