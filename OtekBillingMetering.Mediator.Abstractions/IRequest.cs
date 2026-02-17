namespace OtekBillingMetering.Mediator.Abstractions;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;

