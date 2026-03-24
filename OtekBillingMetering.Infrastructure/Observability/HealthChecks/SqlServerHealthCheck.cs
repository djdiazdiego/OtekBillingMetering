using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data;

namespace OtekBillingMetering.Infrastructure.Observability.HealthChecks;

internal sealed class SqlServerHealthCheck : IHealthCheck
{
	private readonly string _connectionString;

	public SqlServerHealthCheck(string connectionString)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
		_connectionString = connectionString;
	}

	public async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			await using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync(cancellationToken);

			return connection.State == ConnectionState.Open
				? HealthCheckResult.Healthy("SQL Server connection is available.")
				: HealthCheckResult.Unhealthy("SQL Server connection could not be opened.");
		}
		catch(Exception ex)
		{
			return HealthCheckResult.Unhealthy(
				description: "SQL Server health check failed.",
				exception: ex);
		}
	}
}