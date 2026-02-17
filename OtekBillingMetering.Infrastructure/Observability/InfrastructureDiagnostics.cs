using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OtekBillingMetering.Infrastructure.Observability;

internal static class InfrastructureDiagnostics
{
	public const string ActivitySourceName = "OtekBillingMetering.Infrastructure";
	public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

	public const string MeterName = "OtekBillingMetering.Infrastructure";
	public static readonly Meter Meter = new(MeterName);

	public static readonly Histogram<double> RepositoryDurationMs =
		Meter.CreateHistogram<double>("otek.repo.duration_ms", unit: "ms");
}
