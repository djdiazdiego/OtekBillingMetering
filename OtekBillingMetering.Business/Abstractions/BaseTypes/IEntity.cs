namespace OtekBillingMetering.Business.Abstractions.BaseTypes;

public interface IEntity
{
	object Id { get; }
	DateTimeOffset CreationDate { get; }
	DateTimeOffset? LastUpdateDate { get; }
	byte[] Version { get;}
	bool IsTransient();
}
