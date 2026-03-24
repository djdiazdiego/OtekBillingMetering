using OtekBillingMetering.Business.Models.BillingModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.BillingReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.BillingReadRepositories;

internal sealed class BillingCompanyReadRepository(ReadDbContext dbContext)
	: ReadRepository<BillingCompany>(dbContext), IBillingCompanyReadRepository;
