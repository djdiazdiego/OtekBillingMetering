using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OtekBillingMetering.Execution.Behaviors;
using OtekBillingMetering.Execution.Behaviors.Extensions;
using OtekBillingMetering.Mediator;

namespace OtekBillingMetering.Execution;

public static class ExecutionServiceCollectionExtensions
{
	public static IServiceCollection AddExecution(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddLoggingBehaviorOptions(configuration);

		var currrentAssembly = typeof(ExecutionServiceCollectionExtensions).Assembly;

		services.AddValidatorsFromAssembly(currrentAssembly);
		ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
		ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

		services.AddMediator(opt =>
		{
			opt.RegisterFromAssemblies(currrentAssembly);
			opt.AddOpenBehavior(typeof(LoggingBehavior<,>));
			opt.AddOpenBehavior(typeof(ValidationBehavior<,>));
		});

		return services;
	}
}
