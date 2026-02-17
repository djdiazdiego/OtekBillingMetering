// Ignore spelling: ok

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OtekBillingMetering.Execution.Behaviors.Options;
using OtekBillingMetering.Execution.Observability;
using OtekBillingMetering.Mediator.Abstractions;
using System.Diagnostics;

namespace OtekBillingMetering.Execution.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse>(
	ILogger<LoggingBehavior<TRequest, TResponse>> logger,
	IOptions<LoggingBehaviorOptions> options)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull, IRequest
	where TResponse : notnull
{
	private static readonly EventId StartEvent = new(1000, "MediatorRequestStart");
	private static readonly EventId EndEvent = new(1001, "MediatorRequestEnd");
	private static readonly EventId SlowEvent = new(1002, "MediatorRequestSlow");
	private static readonly EventId CancelledEvent = new(1003, "MediatorRequestCancelled");
	private static readonly EventId ErrorEvent = new(1004, "MediatorRequestError");

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var requestType = typeof(TRequest).Name;
		var responseType = typeof(TResponse).Name;

		var opt = options.Value;
		var startEndLevel = opt.LogStartEndAsInformation ? LogLevel.Information : LogLevel.Debug;

		var start = Stopwatch.GetTimestamp();

		using var activity = ExecutionDiagnostics.ActivitySource.StartActivity(
			name: $"Mediator {requestType}",
			kind: ActivityKind.Internal);

		activity?.SetTag("otek.mediator.request_type", requestType);
		activity?.SetTag("otek.mediator.response_type", responseType);

		var traceId = activity?.TraceId.ToString() ?? Activity.Current?.TraceId.ToString();
		var spanId = activity?.SpanId.ToString() ?? Activity.Current?.SpanId.ToString();

		using var scope = logger.BeginScope(
			"TraceId={TraceId} SpanId={SpanId} RequestType={RequestType} ResponseType={ResponseType}",
			traceId, spanId, requestType, responseType);

		if(logger.IsEnabled(startEndLevel))
		{
			logger.Log(startEndLevel, StartEvent, "[START] Handling request.");
		}

		try
		{
			var response = await next(cancellationToken).ConfigureAwait(false);

			var elapsedMs = ElapsedMilliseconds(start);

			activity?.SetTag("otek.mediator.elapsed_ms", elapsedMs);

			var isSlow = elapsedMs > opt.SlowRequestThresholdMs;
			var outcome = isSlow ? "slow" : "ok";

			activity?.SetTag("otek.mediator.outcome", outcome);

			ExecutionDiagnostics.MediatorDurationMs.Record(
				elapsedMs,
				new KeyValuePair<string, object?>("request_type", requestType),
				new KeyValuePair<string, object?>("outcome", outcome));

			if(isSlow)
			{
				logger.LogWarning(SlowEvent, "[PERFORMANCE] Slow request: {ElapsedMs} ms.", elapsedMs);
				activity?.SetStatus(ActivityStatusCode.Ok, description: "slow");
			}
			else
			{
				activity?.SetStatus(ActivityStatusCode.Ok);
			}

			if(logger.IsEnabled(startEndLevel))
			{
				logger.Log(startEndLevel, EndEvent, "[END] Handled request in {ElapsedMs} ms.", elapsedMs);
			}

			return response;
		}
		catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested)
		{
			var elapsedMs = ElapsedMilliseconds(start);

			logger.LogWarning(CancelledEvent, "[CANCELLED] Request cancelled after {ElapsedMs} ms.", elapsedMs);

			activity?.SetTag("otek.mediator.elapsed_ms", elapsedMs);
			activity?.SetTag("otek.mediator.outcome", "cancelled");

			activity?.SetStatus(ActivityStatusCode.Unset, "cancelled");

			ExecutionDiagnostics.MediatorDurationMs.Record(
				elapsedMs,
				new KeyValuePair<string, object?>("request_type", requestType),
				new KeyValuePair<string, object?>("outcome", "cancelled"));

			throw;
		}
		catch(Exception ex)
		{
			var elapsedMs = ElapsedMilliseconds(start);

			logger.LogError(ErrorEvent, ex, "[ERROR] Unhandled exception after {ElapsedMs} ms.", elapsedMs);

			activity?.SetTag("otek.mediator.elapsed_ms", elapsedMs);
			activity?.SetTag("otek.mediator.outcome", "error");

			activity?.SetStatus(ActivityStatusCode.Error);
			activity?.AddException(ex);

			ExecutionDiagnostics.MediatorDurationMs.Record(
				elapsedMs,
				new KeyValuePair<string, object?>("request_type", requestType),
				new KeyValuePair<string, object?>("outcome", "error"));

			throw;
		}
	}

	private static double ElapsedMilliseconds(long startTimestamp)
	{
		var end = Stopwatch.GetTimestamp();
		return (end - startTimestamp) * 1000.0 / Stopwatch.Frequency;
	}
}
