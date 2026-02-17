namespace OtekBillingMetering.Mediator.Abstractions;

public interface INotificationHandler<in TRequest>
	where TRequest : INotification
{
	Task Publish(TRequest request, CancellationToken cancellationToken);
}
