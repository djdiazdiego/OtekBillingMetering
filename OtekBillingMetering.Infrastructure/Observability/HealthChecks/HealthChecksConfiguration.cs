using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace OtekBillingMetering.Infrastructure.Observability.HealthChecks;

public static class HealthChecksConfiguration
{
	public static IServiceCollection AddAppHealthChecks(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(configuration);

		var connectionString = configuration.GetConnectionString(InfrastructureConstants.CONNECTION_STRING_NAME);

		if(string.IsNullOrWhiteSpace(connectionString))
		{
			throw new InvalidOperationException(
				"The connection string 'DefaultConnection' is missing or empty.");
		}

		services
			.AddHealthChecks()
			.AddCheck(
				name: "self",
				check: () => HealthCheckResult.Healthy("Application is running."),
				tags: ["live"])
			.AddCheck(
				name: "sqlserver",
				instance: new SqlServerHealthCheck(connectionString),
				failureStatus: HealthStatus.Unhealthy,
				tags: ["ready", "db", "sqlserver"]);

		return services;
	}

	public static IEndpointRouteBuilder MapAppHealthChecks(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
		{
			Predicate = registration => registration.Tags.Contains("live"),
			ResponseWriter = WriteJsonResponse
		})
		.AllowAnonymous();

		endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
		{
			Predicate = registration => registration.Tags.Contains("ready"),
			ResponseWriter = WriteJsonResponse
		})
		.AllowAnonymous();

		return endpoints;
	}

	private static Task WriteJsonResponse(HttpContext context, HealthReport report)
	{
		context.Response.ContentType = "application/json; charset=utf-8";

		var payload = new
		{
			status = report.Status.ToString(),
			totalDuration = report.TotalDuration,
			entries = report.Entries.Select(entry => new
			{
				name = entry.Key,
				status = entry.Value.Status.ToString(),
				description = entry.Value.Description,
				duration = entry.Value.Duration,
				error = entry.Value.Exception?.Message,
				tags = entry.Value.Tags
			})
		};

		return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
	}
}