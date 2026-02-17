using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Common.Types.DateTime;
using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.RateModels.Types;
using OtekBillingMetering.Business.ValueObjects.RateTiers;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtekBillingMetering.Business.Models.RateModels;

public sealed class RateTier : Entity<Guid>
{
	private RateTier() : base() { }

	internal RateTier(Guid rateId, RateTierDefinition definition) : base(Guid.NewGuid())
	{
		RateId = rateId;
		Apply(definition);
	}

	public string Name { get; private set; } = null!;
	public RateTierType RateTierType { get; private set; }
	public double Multiplier { get; private set; }
	public double? UnitsPerCharge { get; private set; }
	public AtomicUnitType UnitType { get; private set; }
	public double? From { get; private set; }
	public double? To { get; private set; }

	public PercentageBaseTierNames? PercentageBase { get; private set; }

	public MonthType? MonthFrom { get; private set; }
	public MonthType? MonthTo { get; private set; }
	public int? DayOfMonthFrom { get; private set; }
	public int? DayOfMonthTo { get; private set; }
	public WeekdayType? WeekdayFrom { get; private set; }
	public WeekdayType? WeekdayTo { get; private set; }
	public TimeOnly? TimeOfDayFrom { get; private set; }
	public TimeOnly? TimeOfDayTo { get; private set; }

	public Guid? TenantId { get; private set; }
	public Guid RateId { get; private set; }

	public Rate Rate { get; private set; } = null!;

	[NotMapped]
	public bool IsTimeRelated =>
		MonthFrom.HasValue || DayOfMonthFrom.HasValue || WeekdayFrom.HasValue || TimeOfDayFrom.HasValue;

	[NotMapped]
	internal bool IsPercentageTier =>
		RateTierType is RateTierType.TimeRelatedPercentage || RateTierType is RateTierType.Percentage;

	internal void Update(RateTierDefinition definition) => Apply(definition);

	internal void ChangeName(string name) => UpdateName(name);

	internal void ChangePricing(double multiplier, double? unitsPerCharge, AtomicUnitType unitType)
	{
		UpdateMultiplier(multiplier);
		UpdateUnitsPerCharge(unitsPerCharge);
		UpdateMeasurementUnit(unitType);
		ValidateLocalInvariants();
	}

	internal void ChangeMultiplier(double multiplier) => UpdateMultiplier(multiplier);

	internal void ChangeUnitsPerCharge(double? unitsPerCharge) => UpdateUnitsPerCharge(unitsPerCharge);

	internal void ChangeMeasurementUnit(AtomicUnitType unitType)
	{
		UpdateMeasurementUnit(unitType);
		ValidateLocalInvariants();
	}

	internal void ChangeRange(double? from, double? to) => UpdateRange(from, to);

	internal void SetPercentageBase(IEnumerable<string> targetTierNames)
	{
		if(!IsPercentageTier)
		{
			throw new DomainValidationException("Only percentage tiers can define a percentage base.");
		}

		PercentageBase = PercentageBaseTierNames.Create(targetTierNames, Name);
	}

	internal void ChangeTimeWindow(
		MonthType? monthFrom, MonthType? monthTo,
		int? dayFrom, int? dayTo,
		WeekdayType? weekdayFrom, WeekdayType? weekdayTo,
		TimeOnly? timeFrom, TimeOnly? timeTo)
	{
		UpdateTimeWindow(
			monthFrom, monthTo,
			dayFrom, dayTo,
			weekdayFrom, weekdayTo,
			timeFrom, timeTo
		);

		ValidateLocalInvariants();
	}

	internal void AddToAccount(Guid tenantId)
	{
		if(TenantId.HasValue)
		{
			throw new DomainConflictException("RateTier is already assigned to an account.");
		}

		if(tenantId == default)
		{
			throw new DomainValidationException("TenantId cannot be empty.");
		}

		TenantId = tenantId;
	}

	private void Apply(RateTierDefinition definition)
	{
		UpdateName(definition.Name);
		UpdateType(definition.RateTierType);

		UpdateMultiplier(definition.Multiplier);
		UpdateMeasurementUnit(definition.UnitType);
		UpdateUnitsPerCharge(definition.UnitsPerCharge);

		UpdateRange(definition.From, definition.To);

		UpdateTimeWindow(
			definition.MonthFrom, definition.MonthTo,
			definition.DayOfMonthFrom, definition.DayOfMonthTo,
			definition.WeekdayFrom, definition.WeekdayTo,
			definition.TimeOfDayFrom, definition.TimeOfDayTo
		);

		ValidateLocalInvariants();
	}

	private void UpdateName(string name)
	{
		if(string.IsNullOrWhiteSpace(name))
		{
			throw new DomainValidationException("RateTier name is required.");
		}

		Name = name.Trim();
	}

	private void UpdateType(RateTierType type) => RateTierType = type;

	private void UpdateMultiplier(double multiplier)
	{
		if(multiplier <= 0)
		{
			throw new DomainValidationException("Multiplier must be > 0.");
		}

		Multiplier = multiplier;
	}

	private void UpdateUnitsPerCharge(double? unitsPerCharge)
	{
		if(unitsPerCharge.HasValue && unitsPerCharge.Value < 0)
		{
			throw new DomainValidationException("UnitsPerCharge cannot be negative.");
		}

		UnitsPerCharge = unitsPerCharge;
	}

	private void UpdateMeasurementUnit(AtomicUnitType unitType)
		=> UnitType = unitType;

	private void UpdateRange(double? from, double? to)
	{
		if(!RequiresNumericRange())
		{
			throw new DomainValidationException("This tier type does not support ranges.");
		}

		From = from;
		To = to;

		if(!From.HasValue)
		{
			throw new DomainValidationException("From is required for ranged tiers.");
		}

		if(To.HasValue && From.Value >= To.Value)
		{
			throw new DomainValidationException("From must be < To when To is provided.");
		}
	}

	private void UpdateTimeWindow(
		MonthType? monthFrom, MonthType? monthTo,
		int? dayFrom, int? dayTo,
		WeekdayType? weekdayFrom, WeekdayType? weekdayTo,
		TimeOnly? timeFrom, TimeOnly? timeTo)
	{
		if(!RequiresTimeWindow())
		{
			ClearTimeWindow();
			return;
		}

		MonthFrom = monthFrom;
		MonthTo = monthTo;
		DayOfMonthFrom = dayFrom;
		DayOfMonthTo = dayTo;
		WeekdayFrom = weekdayFrom;
		WeekdayTo = weekdayTo;
		TimeOfDayFrom = timeFrom;
		TimeOfDayTo = timeTo;

		EnsurePairParity(MonthFrom, MonthTo, "MonthFrom", "MonthTo");
		EnsurePairParity(DayOfMonthFrom, DayOfMonthTo, "DayOfMonthFrom", "DayOfMonthTo");

		if(DayOfMonthFrom.HasValue)
		{
			EnsureDayOfMonthRange(DayOfMonthFrom.Value, "DayOfMonthFrom");
		}

		if(DayOfMonthTo.HasValue)
		{
			EnsureDayOfMonthRange(DayOfMonthTo.Value, "DayOfMonthTo");
		}

		if(MonthFrom.HasValue && MonthTo.HasValue &&
		   DayOfMonthFrom.HasValue && DayOfMonthTo.HasValue)
		{
			if(MonthFrom.Value == MonthTo.Value && DayOfMonthFrom.Value > DayOfMonthTo.Value)
			{
				throw new DomainValidationException("DayOfMonthFrom must be <= DayOfMonthTo when MonthFrom == MonthTo.");
			}
		}

		if(WeekdayFrom.HasValue && WeekdayTo.HasValue)
		{
			if(WeekdayFrom.Value == WeekdayTo.Value)
			{
				throw new DomainValidationException("WeekdayFrom must be different than WeekdayTo when both are provided.");
			}
		}

		if(WeekdayFrom.HasValue && !WeekdayTo.HasValue)
		{
			throw new DomainValidationException("WeekdayTo must be provided if WeekdayFrom is provided.");
		}

		if(TimeOfDayFrom.HasValue && TimeOfDayTo.HasValue && TimeOfDayFrom.Value >= TimeOfDayTo.Value)
		{
			throw new DomainValidationException("TimeOfDayFrom must be < TimeOfDayTo.");
		}

		if(!TimeOfDayFrom.HasValue && TimeOfDayTo.HasValue)
		{
			throw new DomainValidationException("TimeOfDayFrom must be provided if TimeOfDayTo is provided.");
		}
	}

	private void ValidateLocalInvariants()
	{
		if(RequiresTimeWindow() && !IsTimeRelated)
		{
			throw new DomainValidationException("This tier type requires time constraints.");
		}
	}

	private void ClearTimeWindow()
	{
		MonthFrom = null;
		MonthTo = null;
		DayOfMonthFrom = null;
		DayOfMonthTo = null;
		WeekdayFrom = null;
		WeekdayTo = null;
		TimeOfDayFrom = null;
		TimeOfDayTo = null;
	}

	private bool RequiresNumericRange() =>
		RateTierType is RateTierType.RangeUsage or RateTierType.TimeRelatedRangeUsage;

	private bool RequiresTimeWindow() =>
		RateTierType is RateTierType.TimeRelatedFlat or
		RateTierType.TimeRelatedRangeUsage or
		RateTierType.TimeRelatedFlatUsage or
		RateTierType.TimeRelatedPercentage;

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
