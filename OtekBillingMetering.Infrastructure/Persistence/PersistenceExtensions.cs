using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OtekBillingMetering.Execution.Abstractions.Persistence;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Interceptors;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Read;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Write;

namespace OtekBillingMetering.Infrastructure.Persistence;

internal static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var migrationsAssembly = typeof(WriteDbContext).Assembly;
		var cs = configuration.GetConnectionString(PersistenceConstants.CONNECTION_STRING)
			?? throw new InvalidOperationException("Missing connection string.");

		services.AddScoped<ISaveChangesInterceptor, PublishDomainEventsInterceptor>();

		services.AddDbContextFactory<WriteDbContext>((sp, opt) =>
		{
#if DEBUG
			opt.EnableSensitiveDataLogging();
			opt.EnableDetailedErrors();
#endif

			opt.UseSqlServer(cs, b => b.MigrationsAssembly(migrationsAssembly));

			var interceptors = sp.GetServices<ISaveChangesInterceptor>()
				.Cast<IInterceptor>()
				.ToArray();

			if(interceptors.Length > 0)
			{
				opt.AddInterceptors(interceptors);
			}
		});

		services.AddDbContextFactory<ReadDbContext>((sp, opt) =>
		{
#if DEBUG
			opt.EnableSensitiveDataLogging();
			opt.EnableDetailedErrors();
#endif

			opt.UseSqlServer(cs);
		});

		services.AddScoped<IUnitOfWork, WriteDbContext>();
		services.AddReadRepositories();
		services.AddWriteRepositories();

		return services;
	}

	public static IServiceCollection AddReadRepositories(this IServiceCollection services)
	{
		services.AddScoped<IAccountReadRepository, AccountReadRepository>();
		return services;
	}

	public static IServiceCollection AddWriteRepositories(this IServiceCollection services)
	{
		services.AddScoped<IAccountWriteRepository, AccountWriteRepository>();
		return services;
	}
}
