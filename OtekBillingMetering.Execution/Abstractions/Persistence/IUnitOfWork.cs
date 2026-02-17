namespace OtekBillingMetering.Execution.Abstractions.Persistence;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
