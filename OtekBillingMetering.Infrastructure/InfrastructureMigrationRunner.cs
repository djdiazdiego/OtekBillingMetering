using Microsoft.Extensions.Configuration;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;

namespace OtekBillingMetering.Infrastructure;

public static class InfrastructureMigrationRunner
{
	public static async Task ApplyMigrationAsync(
		IConfiguration configuration,
		CancellationToken cancellationToken = default)
			=> await DbSchemaMigrator.MigrateAsync(configuration, cancellationToken);
}
