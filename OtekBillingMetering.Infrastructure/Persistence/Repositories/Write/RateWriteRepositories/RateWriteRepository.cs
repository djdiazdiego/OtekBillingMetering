using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.RateWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.RateWriteRepositories;

internal sealed class RateWriteRepository(WriteDbContext dbContext)
	: WriteRepository<Rate>(dbContext), IRateWriteRepository;
