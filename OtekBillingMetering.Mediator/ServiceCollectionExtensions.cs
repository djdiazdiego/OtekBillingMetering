using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OtekBillingMetering.Mediator.Abstractions;
using System.Reflection;

namespace OtekBillingMetering.Mediator;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMediator(
		this IServiceCollection services,
		Action<MediatorServiceConfiguration> configure)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(configure);

		var cfg = new MediatorServiceConfiguration();
		configure(cfg);

		services.TryAddTransient<IMediator, Mediator>();

		if(cfg.AssembliesToRegister.Count == 0)
		{
			throw new InvalidOperationException("No assemblies were registered for mediator handlers. " +
				"Please register at least one assembly using 'RegisterFromAssemblies' or 'RegisterFromType' " +
				"methods on MediatorServiceConfiguration.");
		}

		var assemblies = cfg.AssembliesToRegister.Distinct().ToArray();

		RegisterHandlers(services, assemblies);
		RegisterBehaviors(services, cfg.BehaviorsToRegister);

		return services;
	}

	private static void RegisterBehaviors(IServiceCollection services, List<ServiceDescriptor> behaviors)
	{
		foreach(var descriptor in behaviors)
		{
			services.TryAddEnumerable(descriptor);
		}
	}

	private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies)
	{
		var voidHandler = typeof(IRequestHandler<>);
		var notificationHandler = typeof(INotificationHandler<>);
		var handler = typeof(IRequestHandler<,>);

		foreach(var type in assemblies.SelectMany(a => a.DefinedTypes))
		{
			if(type.IsAbstract || type.IsInterface)
			{
				continue;
			}

			foreach(var itf in type.ImplementedInterfaces)
			{
				if(!itf.IsGenericType)
				{
					continue;
				}

				var def = itf.GetGenericTypeDefinition();

				if(def == voidHandler || def == handler || def == notificationHandler)
				{
					services.TryAddEnumerable(ServiceDescriptor.Transient(itf, type.AsType()));
				}
			}
		}
	}
}
