using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;

namespace OtekBillingMetering.Business.Policies.Billing;

public sealed record BillingPolicy(
	UtilityType Utility,
	AtomicUnitType BaseUsageUnitCode,
	double BaseResolution)
{
	public const double ComparisonTolerance = 1e-9;
	public const double QuantizationTolerance = 1e-12;
	public const long MaxCents = 9_000_000_000_000_000_000L;

	public static readonly BillingPolicy Water = new(
		Utility: UtilityType.Water,
		BaseUsageUnitCode: AtomicUnitType.UsGallon,
		BaseResolution: 0.0001
	);

	public static readonly BillingPolicy Electricity = new(
		Utility: UtilityType.Electricity,
		BaseUsageUnitCode: AtomicUnitType.KilowattHour,
		BaseResolution: 0.001
	);

	public static readonly BillingPolicy Gas = new(
		Utility: UtilityType.Gas,
		BaseUsageUnitCode: AtomicUnitType.CubicFoot,
		BaseResolution: 0.01
	);

	public static readonly BillingPolicy Steam = new(
		Utility: UtilityType.Steam,
		BaseUsageUnitCode: AtomicUnitType.Pound,
		BaseResolution: 0.01
	);

	public static BillingPolicy FromUtility(UtilityType utility) => utility switch
	{
		UtilityType.Water => Water,
		UtilityType.Electricity => Electricity,
		UtilityType.Gas => Gas,
		UtilityType.Steam => Steam,
		_ => throw new DomainCompatibilityException(
			"Unsupported UtilityType '{0}'. This may be legacy/stale data that requires migration.", utility)
	};
}
