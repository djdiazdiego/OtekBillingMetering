using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OtekBillingMetering.Infrastructure.Observability.HealthChecks;

public static class HealthChecksExtensions
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
}