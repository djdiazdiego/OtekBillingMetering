using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Mediator.Abstractions;

namespace OtekBillingMetering.Infrastructure.Persistence.Interceptors;

internal sealed class PublishDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
	private static readonly AsyncLocal<bool> _publishing = new();

	public override async ValueTask<int> SavedChangesAsync(
		SaveChangesCompletedEventData eventData,
		int result,
		CancellationToken cancellationToken = default)
	{
		var context = ValidateDbContext(eventData.Context);

		if(_publishing.Value)
		{
			return await base.SavedChangesAsync(eventData, result, cancellationToken)
				.ConfigureAwait(false);
		}

		_publishing.Value = true;

		try
		{
			while(true)
			{
				var domainEvents = CollectAndClearDomainEvents(context);
				if(domainEvents.Count == 0)
				{
					break;
				}

				foreach(var domainEvent in domainEvents)
				{
					await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
				}
			}

			return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
		}
		finally
		{
			_publishing.Value = false;
		}
	}

	private static List<INotification> CollectAndClearDomainEvents(DbContext context)
	{
		var aggregates = context.ChangeTracker.Entries<IAggregateRoot>()
			.Select(e => e.Entity)
			.ToList();

		var domainEvents = new List<INotification>();

		foreach(var aggregate in aggregates)
		{
			if(aggregate is null)
			{
				continue;
			}

			var events = aggregate.GetDomainEvents();
			if(events == null || events.Count == 0)
			{
				continue;
			}

			domainEvents.AddRange(events);
			aggregate.ClearDomainEvents();
		}

		return domainEvents;
	}

	private static WriteDbContext ValidateDbContext(DbContext? context)
	=> context is WriteDbContext write
		? write
		: throw new InvalidOperationException("Interceptor attached to a non-write DbContext.");
}
