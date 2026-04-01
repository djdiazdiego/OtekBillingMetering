using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace OtekBillingMetering.Infrastructure.Persistence.DbContexts;

internal static class DbSchemaMigrator
{
	public static async Task MigrateAsync(IConfiguration configuration, CancellationToken cancellationToken = default)
	{
		var connectionString = configuration.GetConnectionString(InfrastructureConstants.CONNECTION_STRING_NAME)
			?? throw new InvalidOperationException($"Connection string {InfrastructureConstants.CONNECTION_STRING_NAME} not found.");

		var options = new DbContextOptionsBuilder<WriteDbContext>()
			.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(WriteDbContext).Assembly))
			.Options;

		await using var context = new WriteDbContext(options);

		if(!string.Equals(
			context.Database.ProviderName,
			"Microsoft.EntityFrameworkCore.InMemory",
			StringComparison.OrdinalIgnoreCase))
		{
			var pending = await context.Database.GetPendingMigrationsAsync(cancellationToken);

			if(pending.Any())
			{
				await context.Database.MigrateAsync(cancellationToken);
			}
		}
	}
}
