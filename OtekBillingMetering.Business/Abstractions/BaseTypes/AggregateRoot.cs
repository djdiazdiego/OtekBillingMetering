using OtekBillingMetering.Mediator.Abstractions;

namespace OtekBillingMetering.Business.Abstractions.BaseTypes;

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
	where TKey : notnull, IEquatable<TKey>
{
	private readonly List<INotification> _domainEvents;

	protected AggregateRoot() => _domainEvents = [];

	protected AggregateRoot(TKey id) : base(id) => _domainEvents = [];

	public void AddDomainEvent(INotification @event) => _domainEvents.Add(@event);

	public void RemoveDomainEvent(INotification @event) => _domainEvents.Remove(@event);

	public void ClearDomainEvents() => _domainEvents.Clear();

	public IReadOnlyCollection<INotification> GetDomainEvents() => _domainEvents;
}

