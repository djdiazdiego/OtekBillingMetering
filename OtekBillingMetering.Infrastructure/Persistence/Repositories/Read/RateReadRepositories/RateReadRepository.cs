using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.RateReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.RateReadRepositories;

internal sealed class RateReadRepository(ReadDbContext dbContext)
	: ReadRepository<Rate>(dbContext), IRateReadRepository;
