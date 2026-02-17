namespace OtekBillingMetering.Execution.Behaviors.Options;

internal sealed class LoggingBehaviorOptions
{
	public long SlowRequestThresholdMs { get; set; }
	public bool LogStartEndAsInformation { get; set; }
}
