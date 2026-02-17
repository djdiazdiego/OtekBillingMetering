using OtekBillingMetering.Mediator.Abstractions;

namespace OtekBillingMetering.Business.Abstractions.BaseTypes;

public interface IAggregateRoot
{
	void AddDomainEvent(INotification @event);
	void RemoveDomainEvent(INotification @event);
	void ClearDomainEvents();
	IReadOnlyCollection<INotification> GetDomainEvents();
}

