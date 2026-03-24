using OtekBillingMetering.Business.Models.MenuItemModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.MenuItemWriteRepositories;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Write.MenuItemWriteRepositories;

internal sealed class MenuItemWriteRepository(WriteDbContext dbContext)
	: WriteRepository<MenuItem>(dbContext), IMenuItemWriteRepository;
