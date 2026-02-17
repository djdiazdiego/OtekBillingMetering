using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Business.Models.RateModels.Types;
using OtekBillingMetering.Business.Policies.Billing;

namespace OtekBillingMetering.Business.Policies.TierValidation;

public static class NonTimeRelatedRangeTiersPolicy
{
	public static void Validate(IReadOnlyCollection<RateTier> tiers, BillingPolicy billingPolicy)
	{
		var group = tiers
			.Where(t => t.RateTierType == RateTierType.RangeUsage)
			.ToList();

		RangeTierChainPolicy.ValidateChain(
			group: group,
			billingPolicy: billingPolicy,
			groupLabel: "NonTimeRelated RangeUsage"
		);
	}
}