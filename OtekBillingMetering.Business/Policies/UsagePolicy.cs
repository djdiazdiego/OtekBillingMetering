using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Common.Types;
using OtekBillingMetering.Business.Policies.Billing;

namespace OtekBillingMetering.Business.Policies;

public static class UsagePolicy
{
	public static double NormalizeUsage(
		double usage,
		BillingPolicy policy,
		RoundingModeType mode = RoundingModeType.Down)
	{
		if(!double.IsFinite(usage))
		{
			throw new DomainValidationException("Usage must be finite. Got {0}.", usage);
		}

		if(usage < 0)
		{
			throw new DomainValidationException("Usage cannot be negative.");
		}

		var step = policy.BaseResolution;

		return !double.IsFinite(step) || step <= 0
			? throw new DomainCompatibilityException(
				"BillingPolicy.BaseResolution must be finite and > 0. Possible legacy/stale configuration.")
			: FloatingPointPolicy.IsQuantizedToStep(
				value: usage,
				step: step,
				tolerance: BillingPolicy.QuantizationTolerance)
			? usage
			: mode switch
			{
				RoundingModeType.Down => FloatingPointPolicy.QuantizeDown(usage, step),
				RoundingModeType.Up => FloatingPointPolicy.QuantizeUp(usage, step),
				RoundingModeType.Nearest => FloatingPointPolicy.QuantizeNearest(usage, step),
				RoundingModeType.HalfEven => FloatingPointPolicy.QuantizeHalfEven(usage, step),
				_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown usage quantization mode.")
			};
	}
}
