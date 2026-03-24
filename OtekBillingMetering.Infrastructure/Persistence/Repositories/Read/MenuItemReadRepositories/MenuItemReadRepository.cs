using OtekBillingMetering.Business.Models.MenuItemModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Read.MenuItemReadRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Read.MenuItemReadRepositories;

internal sealed class MenuItemReadRepository(ReadDbContext dbContext)
	: ReadRepository<MenuItem>(dbContext), IMenuItemReadRepository;
