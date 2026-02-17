namespace OtekBillingMetering.Business.Common.Types;

public enum RoundingModeType
{
	Nearest, // half away from zero
	Down, // floor (toward -infinity)
	Up, // ceil (toward +infinity)
	HalfEven // bankers rounding
}