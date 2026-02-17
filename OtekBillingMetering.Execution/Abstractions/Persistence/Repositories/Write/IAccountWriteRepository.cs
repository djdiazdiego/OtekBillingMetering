using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Base;

namespace OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write;

public interface IAccountWriteRepository : IWriteRepository<Account>;
