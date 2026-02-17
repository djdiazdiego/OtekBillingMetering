using Microsoft.EntityFrameworkCore;
using OtekBillingMetering.Execution.Abstractions.Persistence;

namespace OtekBillingMetering.Infrastructure.Persistence.DbContexts;

internal sealed class WriteDbContext(DbContextOptions<WriteDbContext> options) : AppDbContextBase(options), IUnitOfWork;
