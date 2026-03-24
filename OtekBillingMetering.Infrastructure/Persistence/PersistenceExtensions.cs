using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OtekBillingMetering.Execution.Abstractions.Persistence;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.BillingReadRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.IdentityReadRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.MenuItemReadRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.RateReadRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.UtilityReadRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.BillingWriteRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.IdentityWriteRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.MenuItemWriteRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.RateWriteRepositories;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.UtilityWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Interceptors;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.BillingReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.IdentityReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.MenuItemReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.RateReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.UtilityReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.BillingWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.IdentityWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.MenuItemWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.RateWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.UtilityWriteRepositories;

namespace OtekBillingMetering.Infrastructure.Persistence;

internal static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var migrationsAssembly = typeof(WriteDbContext).Assembly;
		var cs = configuration.GetConnectionString(InfrastructureConstants.CONNECTION_STRING_NAME)
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
		services.AddScoped<IUserReadRepository, UserReadRepository>();
		services.AddScoped<IRoleReadRepository, RoleReadRepository>();
		services.AddScoped<IPolicyReadRepository, PolicyReadRepository>();
		services.AddScoped<IGroupReadRepository, GroupReadRepository>();
		services.AddScoped<IBillingCompanyReadRepository, BillingCompanyReadRepository>();
		services.AddScoped<IBillingCompanyClientReadRepository, BillingCompanyClientReadRepository>();
		services.AddScoped<IUtilityReadRepository, UtilityReadRepository>();
		services.AddScoped<IRateReadRepository, RateReadRepository>();
		services.AddScoped<IRateTierReadRepository, RateTierReadRepository>();
		services.AddScoped<IMenuItemReadRepository, MenuItemReadRepository>();
		return services;
	}

	public static IServiceCollection AddWriteRepositories(this IServiceCollection services)
	{
		services.AddScoped<IAccountWriteRepository, AccountWriteRepository>();
		services.AddScoped<IUserWriteRepository, UserWriteRepository>();
		services.AddScoped<IRoleWriteRepository, RoleWriteRepository>();
		services.AddScoped<IPolicyWriteRepository, PolicyWriteRepository>();
		services.AddScoped<IGroupWriteRepository, GroupWriteRepository>();
		services.AddScoped<IBillingCompanyWriteRepository, BillingCompanyWriteRepository>();
		services.AddScoped<IUtilityWriteRepository, UtilityWriteRepository>();
		services.AddScoped<IRateWriteRepository, RateWriteRepository>();
		services.AddScoped<IMenuItemWriteRepository, MenuItemWriteRepository>();
		return services;
	}
}
