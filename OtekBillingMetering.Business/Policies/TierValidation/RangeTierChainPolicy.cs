using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Business.Policies.Billing;

namespace OtekBillingMetering.Business.Policies.TierValidation;

public static class RangeTierChainPolicy
{
	public static void ValidateChain(
		IReadOnlyCollection<RateTier> group,
		BillingPolicy billingPolicy,
		string groupLabel)
	{
		var ranged = group
			.OrderBy(t => t.From ?? double.NaN)
			.ToList();

		if(ranged.Count == 0)
		{
			return;
		}

		if(ranged.Any(t => !t.From.HasValue))
		{
			throw new DomainValidationException("{0}: all ranged tiers must have From.", groupLabel);
		}

		foreach(var t in ranged)
		{
			var from = t.From!.Value;

			if(!FloatingPointPolicy.IsQuantizedToStep(
				   value: from,
				   step: billingPolicy.BaseResolution,
				   tolerance: BillingPolicy.QuantizationTolerance))
			{
				throw new DomainValidationException(
					"{0}: tier From ({1}) must be aligned to base resolution ({2}).",
					groupLabel,
					from,
					billingPolicy.BaseResolution
				);
			}

			if(t.To.HasValue)
			{
				var to = t.To.Value;

				if(!FloatingPointPolicy.IsQuantizedToStep(
					   value: to,
					   step: billingPolicy.BaseResolution,
					   tolerance: BillingPolicy.QuantizationTolerance))
				{
					throw new DomainValidationException(
						"{0}: tier To ({1}) must be aligned to base resolution ({2}).",
						groupLabel,
						to,
						billingPolicy.BaseResolution
					);
				}
			}
		}

		var zeroCount = ranged.Count(t =>
			FloatingPointPolicy.AreApproximatelyEqual(
				t.From!.Value,
				0.0,
				BillingPolicy.ComparisonTolerance));

		if(zeroCount == 0)
		{
			throw new DomainValidationException("{0}: range tiers must include a tier with From = 0.", groupLabel);
		}

		if(zeroCount > 1)
		{
			throw new DomainValidationException("{0}: range tiers cannot contain multiple tiers with From = 0.", groupLabel);
		}

		for(var i = 0; i < ranged.Count; i++)
		{
			var current = ranged[i];
			var isLast = i == ranged.Count - 1;

			if(isLast)
			{
				if(current.To.HasValue)
				{
					throw new DomainValidationException(
					"{0}: last tier must have To = null, but it has To = {1}.",
					groupLabel,
					current.To.Value);
				}
				continue;
			}

			if(!current.To.HasValue)
			{
				throw new DomainValidationException(
					"{0}: tier with From = {1} must have To because it is not the last tier.",
					groupLabel,
					current.From!.Value
				);
			}

			var next = ranged[i + 1];

			if(!FloatingPointPolicy.AreApproximatelyEqual(
				   current.To.Value,
				   next.From!.Value,
				   BillingPolicy.ComparisonTolerance))
			{
				throw new DomainValidationException(
					"{0}: tiers must be contiguous: tier [{1}, {2}] must end where next tier starts (next.From = {3}).",
					groupLabel,
					current.From!.Value,
					current.To.Value,
					next.From!.Value
				);
			}
		}

		if(ranged.Take(ranged.Count - 1).Any(t => t.To is null))
		{
			throw new DomainValidationException("{0}: only the last tier may have To = null.", groupLabel);
		}
	}
}
