namespace OtekBillingMetering.Mediator.Abstractions;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken ct);

public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : IRequest
{
	Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken);
}
