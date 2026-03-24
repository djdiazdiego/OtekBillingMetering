using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Base;

namespace OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Write.RateWriteRepositories;

public interface IRateWriteRepository : IWriteRepository<Rate>;
