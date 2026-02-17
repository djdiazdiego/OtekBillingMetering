using OtekBillingMetering.Business.Common.Types.DateTime;
using OtekBillingMetering.Business.Models.RateModels;

namespace OtekBillingMetering.Business.ValueObjects.RateTiers;

public sealed record RateTierTimeWindowKey(
	MonthType? MonthFrom,
	MonthType? MonthTo,
	int? DayOfMonthFrom,
	int? DayOfMonthTo,
	WeekdayType? WeekdayFrom,
	WeekdayType? WeekdayTo,
	TimeOnly? TimeOfDayFrom,
	TimeOnly? TimeOfDayTo)
{
	public static RateTierTimeWindowKey FromDefinition(RateTierDefinition d) => new(
		d.MonthFrom, d.MonthTo,
		d.DayOfMonthFrom, d.DayOfMonthTo,
		d.WeekdayFrom, d.WeekdayTo,
		d.TimeOfDayFrom, d.TimeOfDayTo
	);

	public static RateTierTimeWindowKey FromTier(RateTier t) => new(
		t.MonthFrom, t.MonthTo,
		t.DayOfMonthFrom, t.DayOfMonthTo,
		t.WeekdayFrom, t.WeekdayTo,
		t.TimeOfDayFrom, t.TimeOfDayTo
	);
}