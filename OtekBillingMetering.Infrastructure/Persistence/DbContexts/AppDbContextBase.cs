using Microsoft.EntityFrameworkCore;
using OtekBillingMetering.Business.Models.Billing;
using OtekBillingMetering.Business.Models.BillingModels;
using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Business.Models.UtilityModels;
using OtekBillingMetering.Infrastructure.Configuration.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.DbContexts;

internal abstract class AppDbContextBase(DbContextOptions options) : DbContext(options)
{
	public DbSet<Account> Accounts => Set<Account>();

	public DbSet<User> Users => Set<User>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<Policy> Policies => Set<Policy>();
	public DbSet<Group> Groups => Set<Group>();

	public DbSet<BillingCompany> BillingCompanies => Set<BillingCompany>();
	public DbSet<BillingCompanyClient> BillingCompanyClients => Set<BillingCompanyClient>();
	public DbSet<BillingCompanyClientLink> BillingCompanyClientLinks => Set<BillingCompanyClientLink>();

	public DbSet<Utility> Utilities => Set<Utility>();

	public DbSet<Rate> Rates => Set<Rate>();
	public DbSet<RateTier> RateTiers => Set<RateTier>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(EntityConfigBase<>).Assembly);
	}
}
