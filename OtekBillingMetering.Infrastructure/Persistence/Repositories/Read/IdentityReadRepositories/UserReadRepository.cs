using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.IdentityReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.IdentityReadRepositories;

internal sealed class UserReadRepository(ReadDbContext dbContext)
	: ReadRepository<User>(dbContext), IUserReadRepository;
