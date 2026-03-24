using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OtekBillingMetering.Infrastructure.Observability.Telemetry;
using OtekBillingMetering.Infrastructure.Persistence;

namespace OtekBillingMetering.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddPersistence(configuration);
		services.AddObservability(configuration);

		return services;
	}
}
