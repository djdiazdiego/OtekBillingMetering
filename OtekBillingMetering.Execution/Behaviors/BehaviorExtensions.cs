using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OtekBillingMetering.Execution.Behaviors.Options;

namespace OtekBillingMetering.Execution.Behaviors;

internal static class BehaviorExtensions
{
	public static IServiceCollection AddLoggingBehaviorOptions(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptions<LoggingBehaviorOptions>()
			.Bind(configuration.GetSection("LoggingBehavior"))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		return services;
	}
}
