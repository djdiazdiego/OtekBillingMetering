using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Read;

internal sealed class AccountReadRepository(ReadDbContext dbContext)
	: ReadRepository<Account>(dbContext), IAccountReadRepository;
