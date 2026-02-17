namespace OtekBillingMetering.Mediator.Abstractions;

public interface IMediator
{
	Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
		where TRequest : IRequest<TResponse>;
	Task Dispatch<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
	Task Publish<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : INotification;
	Task Publish(INotification notification, CancellationToken cancellationToken = default);
}
