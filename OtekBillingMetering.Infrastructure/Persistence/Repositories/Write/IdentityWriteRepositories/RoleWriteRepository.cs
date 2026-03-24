using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.IdentityWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.IdentityWriteRepositories;

internal sealed class RoleWriteRepository(WriteDbContext dbContext)
	: WriteRepository<Role>(dbContext), IRoleWriteRepository;
