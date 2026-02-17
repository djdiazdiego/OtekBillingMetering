using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Common.Types;
using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Business.ValueObjects;

namespace OtekBillingMetering.Business.Policies.Billing;

public static class BillingAmountPolicy
{
	public static MoneyCents ComputeUsageDrivenAmount(
		RateTier tier,
		double normalizedUsage,
		BillingPolicy policy,
		Func<RateTier, BillingPolicy, decimal> priceResolver,
		RoundingModeType rounding = RoundingModeType.Down)
	{
		if(tier is null)
		{
			throw new DomainValidationException("Tier is required.");
		}

		if(priceResolver is null)
		{
			throw new DomainValidationException("Price resolver is required.");
		}

		if(!double.IsFinite(normalizedUsage) || normalizedUsage < 0)
		{
			throw new DomainValidationException("Usage must be normalized (finite and >= 0).");
		}

		if(!tier.UnitsPerCharge.HasValue)
		{
			throw new DomainValidationException("UnitsPerCharge is required for this tier type.");
		}

		var unitsPerCharge = tier.UnitsPerCharge.Value;

		if(!double.IsFinite(unitsPerCharge) || unitsPerCharge <= 0)
		{
			throw new DomainValidationException("UnitsPerCharge must be finite and > 0.");
		}

		var charges = ComputeCharges(
			multiplier: tier.Multiplier,
			usage: normalizedUsage,
			usagePerCharge: unitsPerCharge);

		var pricePerCharge = priceResolver(tier, policy);

		if(pricePerCharge < 0)
		{
			throw new DomainValidationException("PricePerCharge cannot be negative.");
		}

		var amountDollars = charges * pricePerCharge;
		var rounded = MoneyCents.FromDollars(amountDollars).RoundedToCents(rounding);
		var cents = rounded.ToCents(rounding);

		return cents is > BillingPolicy.MaxCents or < -BillingPolicy.MaxCents ?
			throw new DomainValidationException(
				"Computed amount exceeds allowed bounds. cents={0}, max={1}.",
				cents,
				BillingPolicy.MaxCents) :
			rounded;
	}

	private static decimal ComputeCharges(double multiplier, double usage, double usagePerCharge)
	{
		if(!double.IsFinite(multiplier) || multiplier <= 0)
		{
			throw new DomainValidationException("Multiplier must be finite and > 0.");
		}

		var raw = multiplier * usage / usagePerCharge;

		return !double.IsFinite(raw) ?
			throw new DomainValidationException("Charges computation overflowed.") :
			(decimal)raw;
	}
}
