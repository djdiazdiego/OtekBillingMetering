using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OtekBillingMetering.Execution.Observability;
using OtekBillingMetering.Infrastructure.Observability.Telemetry.Options;

namespace OtekBillingMetering.Infrastructure.Observability.Telemetry;

internal static class OpenTelemetryExtensions
{
	public static IServiceCollection AddObservability(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var opt = configuration.GetSection("Observability").Get<ObservabilityOptions>()
			?? throw new InvalidOperationException("Observability configuration section is missing or invalid.");

		services.AddOpenTelemetry()
			.ConfigureResource(r =>
			{
				var rb = r.AddService(
					serviceName: opt.ServiceName,
					serviceVersion: opt.ServiceVersion);

				if(!string.IsNullOrWhiteSpace(opt.Environment))
				{
					rb.AddAttributes(
					[
						new KeyValuePair<string, object>("deployment.environment", opt.Environment!)
					]);
				}
			})
			.WithTracing(t =>
			{
				if(!opt.EnableTracing)
				{
					return;
				}

				t.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(opt.TraceSamplingRatio)));

				t.AddAspNetCoreInstrumentation();
				t.AddHttpClientInstrumentation();

				t.AddSqlClientInstrumentation(o =>
				{
					o.RecordException = true;

					o.Filter = obj =>
					{
						if(obj is Microsoft.Data.SqlClient.SqlCommand cmd)
						{
							if(cmd.CommandText?.Contains("SELECT 1", StringComparison.OrdinalIgnoreCase) == true)
							{
								return false;
							}
						}

						return true;
					};

					o.EnrichWithSqlCommand = (activity, obj) =>
					{
						if(obj is Microsoft.Data.SqlClient.SqlCommand cmd)
						{
							activity.SetTag("db.command_timeout", cmd.CommandTimeout);
							activity.SetTag("db.command_type", cmd.CommandType.ToString());
						}
					};
				});

				t.AddSource(ExecutionDiagnostics.ActivitySourceName);
				t.AddSource(InfrastructureDiagnostics.ActivitySourceName);

				t.AddOtlpExporter(o =>
				{
					if(!string.IsNullOrWhiteSpace(opt.OtlpEndpoint))
					{
						o.Endpoint = new Uri(opt.OtlpEndpoint!);
					}
				});
			})
			.WithMetrics(m =>
			{
				if(!opt.EnableMetrics)
				{
					return;
				}

				m.AddAspNetCoreInstrumentation();
				m.AddHttpClientInstrumentation();

				if(opt.EnableRuntimeInstrumentation)
				{
					m.AddRuntimeInstrumentation();
				}

				m.AddMeter(ExecutionDiagnostics.MeterName);
				m.AddMeter(InfrastructureDiagnostics.MeterName);

				m.AddOtlpExporter(o =>
				{
					if(!string.IsNullOrWhiteSpace(opt.OtlpEndpoint))
					{
						o.Endpoint = new Uri(opt.OtlpEndpoint!);
					}
				});
			});

		return services;
	}
}