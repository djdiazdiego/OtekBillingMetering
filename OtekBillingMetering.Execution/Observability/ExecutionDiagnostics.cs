using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OtekBillingMetering.Execution.Observability;

public static class ExecutionDiagnostics
{
	public const string ActivitySourceName = "OtekBillingMetering.Execution.Mediator";

	public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

	public const string MeterName = "OtekBillingMetering.Execution";
	public static readonly Meter Meter = new(MeterName);

	public static readonly Histogram<double> MediatorDurationMs =
		Meter.CreateHistogram<double>("otek.mediator.duration_ms", unit: "ms");
}
