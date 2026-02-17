using Microsoft.Extensions.DependencyInjection;
using OtekBillingMetering.Mediator.Abstractions;
using System.Collections.Concurrent;
using System.Reflection;

namespace OtekBillingMetering.Mediator;

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
	private readonly IServiceProvider _sp = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
	private static readonly ConcurrentDictionary<Type, Func<Mediator, INotification, CancellationToken, Task>>
		_publishCache = new();

	public Task Dispatch<TRequest>(TRequest request, CancellationToken cancellationToken = default)
		where TRequest : IRequest
	{
		ArgumentNullException.ThrowIfNull(request);
		return DispatchInternal(request, cancellationToken);
	}

	public Task<TResponse> Dispatch<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken = default)
		where TRequest : IRequest<TResponse>
	{
		ArgumentNullException.ThrowIfNull(request);
		return DispatchInternal<TRequest, TResponse>(request, cancellationToken);
	}

	public Task Publish<TRequest>(TRequest request, CancellationToken cancellationToken = default)
		where TRequest : INotification
	{
		ArgumentNullException.ThrowIfNull(request);
		return InvokeNotificationHandler(request, cancellationToken);
	}

	public Task Publish(INotification notification, CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(notification);
		return InvokeNotificationHandlerByRuntimeType(notification, ct);
	}

	private async Task<TResponse> DispatchInternal<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken)
		where TRequest : IRequest<TResponse>
	{
		var behaviors = _sp.GetServices<IPipelineBehavior<TRequest, TResponse>>().ToArray();

		Task<TResponse> Terminal(CancellationToken ct) => InvokeHandler<TRequest, TResponse>(request, ct);

		RequestHandlerDelegate<TResponse> next = Terminal;

		for(var i = behaviors.Length - 1; i >= 0; i--)
		{
			var behavior = behaviors[i];
			var currentNext = next;
			next = (ct) => behavior.Handle(request, currentNext, ct);
		}

		return await next(cancellationToken).ConfigureAwait(false);
	}

	private async Task DispatchInternal<TRequest>(
		TRequest request,
		CancellationToken cancellationToken)
		where TRequest : IRequest
	{
		var behaviors = _sp.GetServices<IPipelineBehavior<TRequest, Unit>>().ToArray();

		Task<Unit> Terminal(CancellationToken ct) => InvokeHandler(request, ct);

		RequestHandlerDelegate<Unit> next = Terminal;

		for(var i = behaviors.Length - 1; i >= 0; i--)
		{
			var behavior = behaviors[i];
			var currentNext = next;
			next = (ct) => behavior.Handle(request, currentNext, ct);
		}

		await next(cancellationToken).ConfigureAwait(false);
	}

	private Task<TResponse> InvokeHandler<TRequest, TResponse>(TRequest request, CancellationToken ct)
		where TRequest : IRequest<TResponse>
	{
		var handler = _sp.GetService<IRequestHandler<TRequest, TResponse>>() ?? throw new InvalidOperationException(
			$"No handler registered for request '{typeof(TRequest).FullName}' " +
			$"with response '{typeof(TResponse).FullName}'.");

		return handler.Handle(request, ct);
	}

	private async Task<Unit> InvokeHandler<TRequest>(TRequest request, CancellationToken ct)
		where TRequest : IRequest
	{
		var handler = _sp.GetService<IRequestHandler<TRequest>>() ?? throw new InvalidOperationException(
			$"No handler registered for request '{typeof(TRequest).FullName}'.");

		await handler.Handle(request, ct).ConfigureAwait(false);
		return Unit.Value;
	}

	private async Task InvokeNotificationHandler<TRequest>(TRequest request, CancellationToken ct)
	where TRequest : INotification
	{
		var handlers = _sp.GetServices<INotificationHandler<TRequest>>().ToArray();

		if(handlers.Length == 0)
		{
			throw new InvalidOperationException(
				$"No notification handlers registered for '{typeof(TRequest).FullName}'.");
		}

		foreach(var h in handlers)
		{
			await h.Publish(request, ct).ConfigureAwait(false);
		}
	}

	private Task InvokeNotificationHandlerByRuntimeType(INotification notification, CancellationToken ct)
	{
		var invoker = _publishCache.GetOrAdd(notification.GetType(), static t =>
		{
			var method = typeof(Mediator)
				.GetMethod(nameof(InvokeNotificationHandler), BindingFlags.Instance | BindingFlags.NonPublic)!;
			var generic = method.MakeGenericMethod(t);

			return (Mediator m, INotification n, CancellationToken c) =>
				(Task)generic.Invoke(m, [n, c])!;
		});

		return invoker(this, notification, ct);
	}
}
