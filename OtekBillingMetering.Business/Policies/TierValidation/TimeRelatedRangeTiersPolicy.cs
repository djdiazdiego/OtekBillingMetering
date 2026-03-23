using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Business.Models.RateModels.Types;
using OtekBillingMetering.Business.Policies.Billing;
using OtekBillingMetering.Business.ValueObjects.RateTiers;

namespace OtekBillingMetering.Business.Policies.TierValidation;

public static class TimeRelatedRangeTiersPolicy
{
	public static void Validate(IReadOnlyCollection<RateTier> tiers, BillingPolicy billingPolicy)
	{
		var timeRanges = tiers
			.Where(t => t.RateTierType == RateTierType.RangeUsage && t.IsTimeRelated)
			.ToList();

		if(timeRanges.Count == 0)
		{
			return;
		}

		var groups = timeRanges
			.GroupBy(RateTierTimeWindowKey.FromTier)
			.ToList();

		foreach(var g in groups)
		{
			RangeTierChainPolicy.ValidateChain(
				group: [.. g],
				billingPolicy: billingPolicy,
				groupLabel: $"TimeRelatedRangeUsage group {g.Key}"
			);
		}
	}
}
