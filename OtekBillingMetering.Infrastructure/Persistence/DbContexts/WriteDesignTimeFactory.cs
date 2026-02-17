// Ignore spelling: appsettings

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OtekBillingMetering.Infrastructure.Persistence.DbContexts;

internal sealed class WriteDesignTimeFactory : IDesignTimeDbContextFactory<WriteDbContext>
{
	public WriteDbContext CreateDbContext(string[] args)
	{
		var environment =
		   Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

		var config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false)
			.AddJsonFile($"appsettings.{environment}.json", optional: true)
			.AddEnvironmentVariables()
			.Build();

		var connection = config.GetConnectionString(PersistenceConstants.CONNECTION_STRING)
			?? throw new InvalidOperationException($"Connection string {PersistenceConstants.CONNECTION_STRING} not found.");

		var options = new DbContextOptionsBuilder<WriteDbContext>();

		options.UseSqlServer(connection, b => b.MigrationsAssembly(typeof(WriteDbContext).Assembly));

		return new WriteDbContext(options.Options);
	}
}
