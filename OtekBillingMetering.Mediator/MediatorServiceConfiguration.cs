using Microsoft.Extensions.DependencyInjection;
using OtekBillingMetering.Mediator.Abstractions;
using System.Reflection;

namespace OtekBillingMetering.Mediator;

public sealed class MediatorServiceConfiguration
{
	internal List<Assembly> AssembliesToRegister { get; } = [];
	internal List<ServiceDescriptor> BehaviorsToRegister { get; } = [];

	public MediatorServiceConfiguration RegisterFromAssemblies(params Assembly[] assemblies)
	{
		if(assemblies is null || assemblies.Length == 0)
		{
			throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));
		}

		AssembliesToRegister.AddRange(assemblies);
		return this;
	}

	public MediatorServiceConfiguration RegisterFromType<T>()
	{
		if(typeof(T).Assembly is null)
		{
			throw new InvalidOperationException($"Type {typeof(T).FullName} does not have a valid assembly.");
		}

		AssembliesToRegister.Add(typeof(T).Assembly);
		return this;
	}

	public MediatorServiceConfiguration AddOpenBehavior(
		Type openBehaviorType,
		ServiceLifetime lifetime = ServiceLifetime.Transient)
	{
		ArgumentNullException.ThrowIfNull(openBehaviorType);

		if(!openBehaviorType.IsGenericTypeDefinition)
		{
			throw new InvalidOperationException(
				$"{openBehaviorType.Name} must be an open generic type definition, e.g. LoggingBehavior<,>.");
		}

		if(openBehaviorType.IsAbstract || openBehaviorType.IsInterface)
		{
			throw new InvalidOperationException($"{openBehaviorType.FullName} must be a non-abstract class.");
		}

		if(openBehaviorType.GetGenericArguments().Length != 2)
		{
			throw new InvalidOperationException($"{openBehaviorType.Name} must have two generic parameters.");
		}

		var implementsPipeline = openBehaviorType.GetInterfaces()
			.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

		if(!implementsPipeline)
		{
			throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPipelineBehavior<,>).FullName}.");
		}

		AddDescriptorUnique(new ServiceDescriptor(typeof(IPipelineBehavior<,>), openBehaviorType, lifetime));
		return this;
	}

	public MediatorServiceConfiguration AddBehavior(
		Type behaviorType,
		Type behaviorImplementationType,
		ServiceLifetime lifetime = ServiceLifetime.Transient)
	{
		ArgumentNullException.ThrowIfNull(behaviorType);
		ArgumentNullException.ThrowIfNull(behaviorImplementationType);

		if(!behaviorType.IsGenericType || behaviorType.GetGenericTypeDefinition() != typeof(IPipelineBehavior<,>))
		{
			throw new InvalidOperationException(
				$"{behaviorType.FullName} must be a closed generic of {typeof(IPipelineBehavior<,>).FullName}.");
		}

		if(behaviorImplementationType.IsAbstract || behaviorImplementationType.IsInterface)
		{
			throw new InvalidOperationException($"{behaviorImplementationType.FullName} must be a non-abstract class.");
		}

		var implementsPipeline = behaviorImplementationType
			.GetInterfaces()
			.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

		if(!implementsPipeline)
		{
			throw new InvalidOperationException(
				$"{behaviorImplementationType.FullName} must implement {typeof(IPipelineBehavior<,>).FullName}.");
		}

		if(!behaviorType.IsAssignableFrom(behaviorImplementationType))
		{
			throw new InvalidOperationException(
				$"{behaviorImplementationType.FullName} must implement {behaviorType.FullName}.");
		}

		AddDescriptorUnique(new ServiceDescriptor(behaviorType, behaviorImplementationType, lifetime));
		return this;
	}

	private void AddDescriptorUnique(ServiceDescriptor descriptor)
	{
		var exists = BehaviorsToRegister.Any(d =>
			d.ServiceType == descriptor.ServiceType &&
			d.ImplementationType == descriptor.ImplementationType &&
			d.ImplementationFactory == descriptor.ImplementationFactory &&
			d.ImplementationInstance == descriptor.ImplementationInstance &&
			d.Lifetime == descriptor.Lifetime);

		if(!exists)
		{
			BehaviorsToRegister.Add(descriptor);
		}
	}
}
