using OtekBillingMetering.Business.Abstractions;
using System.Linq.Expressions;

namespace OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Base;

public interface IWriteRepository<TEntity> where TEntity : class, IEntity
{
	Task<TEntity> GetByIdAsync(Guid id, CancellationToken ct);

	Task<TEntity?> GetByIdOrDefaultAsync(Guid id, CancellationToken ct);

	Task<TEntity> GetByFilterAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken ct,
		params string[] includes);

	Task<TEntity?> GetByFilterOrDefaultAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken ct,
		params string[] includes);

	Task<List<TEntity>> GetManyByFilterAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken ct,
		params string[] includes);

	void Remove(TEntity entity);
	void RemoveRange(params TEntity[] entities);
	void Update(TEntity entity);
	void UpdateRange(params TEntity[] entities);
	void Add(TEntity entity);
	void AddRange(params TEntity[] entities);
}
