using Microsoft.EntityFrameworkCore;

namespace OtekBillingMetering.Infrastructure.Persistence.DbContexts;

internal sealed class ReadDbContext(DbContextOptions<ReadDbContext> options) : AppDbContextBase(options)
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);

		ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		ChangeTracker.AutoDetectChangesEnabled = false;
	}

	public override int SaveChanges() =>
		throw new InvalidOperationException("ReadDbContext is read-only. Use WriteDbContext for persistence.");

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
		throw new InvalidOperationException("ReadDbContext is read-only. Use WriteDbContext for persistence.");
}
