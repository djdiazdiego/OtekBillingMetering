using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace OtekBillingMetering.WebApi.Configuration;

internal static class HealthChecksEndpointRouteBuilderExtensions
{
	public static IEndpointRouteBuilder MapAppHealthChecks(
		this IEndpointRouteBuilder endpoints)
	{
		ArgumentNullException.ThrowIfNull(endpoints);

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