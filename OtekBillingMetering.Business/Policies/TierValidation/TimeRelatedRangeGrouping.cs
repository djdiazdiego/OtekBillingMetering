using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Business.Models.RateModels.Types;
using OtekBillingMetering.Business.ValueObjects.RateTiers;

namespace OtekBillingMetering.Business.Policies.TierValidation;

public static class TimeRelatedRangeGrouping
{
	public static IReadOnlyDictionary<RateTierTimeWindowKey, List<RateTier>> Group(
		IReadOnlyCollection<RateTier> tiers) => tiers
			.Where(t => t.RateTierType == RateTierType.TimeRelatedRangeUsage)
			.GroupBy(RateTierTimeWindowKey.FromTier)
			.ToDictionary(g => g.Key, g => g.ToList());
}
