using OtekBillingMetering.Business.Models.UtilityModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.UtilityReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.UtilityReadRepositories;

internal sealed class UtilityReadRepository(ReadDbContext dbContext)
	: ReadRepository<Utility>(dbContext), IUtilityReadRepository;
