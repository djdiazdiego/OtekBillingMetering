// Ignore spelling: Otlp

namespace OtekBillingMetering.Infrastructure.Observability.Options;

internal sealed class ObservabilityOptions
{
	public string ServiceName { get; set; } = null!;
	public string? ServiceVersion { get; set; }
	public string? Environment { get; set; }

	public bool EnableTracing { get; set; }
	public bool EnableMetrics { get; set; }

	public string? OtlpEndpoint { get; set; }
	public double TraceSamplingRatio { get; set; }
	public bool EnableRuntimeInstrumentation { get; set; }
}
