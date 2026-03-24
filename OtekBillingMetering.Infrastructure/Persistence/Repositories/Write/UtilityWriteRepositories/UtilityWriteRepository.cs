using OtekBillingMetering.Business.Models.UtilityModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.UtilityWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.UtilityWriteRepositories;

internal sealed class UtilityWriteRepository(WriteDbContext dbContext)
	: WriteRepository<Utility>(dbContext), IUtilityWriteRepository;
