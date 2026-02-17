using OtekBillingMetering.Business.Policies.Billing;

namespace OtekBillingMetering.Business.Policies;

public static class FloatingPointPolicy
{
	public static bool AreApproximatelyEqual(double a, double b, double tolerance = BillingPolicy.ComparisonTolerance)
		=> Math.Abs(a - b) <= tolerance;

	public static bool LessOrApproximatelyEqual(double a, double b, double tolerance = BillingPolicy.ComparisonTolerance)
		=> a < b || AreApproximatelyEqual(a, b, tolerance);

	public static bool IsQuantizedToStep(
		double value,
		double step,
		double tolerance = BillingPolicy.QuantizationTolerance,
		MidpointRounding rounding = MidpointRounding.ToEven)
	{
		if(!double.IsFinite(value) || !double.IsFinite(step) || step <= 0)
		{
			return false;
		}

		var q = value / step;
		var nearest = Math.Round(q, rounding);
		var snapped = nearest * step;
		return Math.Abs(value - snapped) <= tolerance;
	}

	public static double QuantizeToStep(double value, double step) => !double.IsFinite(value)
		? throw new ArgumentOutOfRangeException(nameof(value), value, "value must be finite.")
		: !double.IsFinite(step) || step <= 0
		? throw new ArgumentOutOfRangeException(nameof(step), step, "step must be finite and > 0.")
		: Math.Round(value / step) * step;

	public static double QuantizeDown(double value, double step)
	{
		EnsureFiniteValue(value);
		EnsureFiniteStep(step);

		return Math.Floor(value / step) * step;
	}

	public static double QuantizeUp(double value, double step)
	{
		EnsureFiniteValue(value);
		EnsureFiniteStep(step);

		return Math.Ceiling(value / step) * step;
	}

	public static double QuantizeNearest(double value, double step)
	{
		EnsureFiniteValue(value);
		EnsureFiniteStep(step);

		return Math.Round(value / step, MidpointRounding.AwayFromZero) * step;
	}

	public static double QuantizeHalfEven(double value, double step)
	{
		EnsureFiniteValue(value);
		EnsureFiniteStep(step);

		return Math.Round(value / step, MidpointRounding.ToEven) * step;
	}

	private static void EnsureFiniteStep(double step)
	{
		if(!double.IsFinite(step) || step <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(step), step, "step must be finite and > 0.");
		}
	}

	private static void EnsureFiniteValue(double value)
	{
		if(!double.IsFinite(value))
		{
			throw new ArgumentOutOfRangeException(nameof(value), value, "value must be finite.");
		}
	}
}
