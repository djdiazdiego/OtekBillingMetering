using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Base;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using System.Linq.Expressions;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

internal abstract class WriteRepository<TEntity>(WriteDbContext dbContext)
	: RepositoryBase<TEntity>(dbContext), IWriteRepository<TEntity>
	where TEntity : class, IEntity
{
	private const bool Tracking = true;

	public Task<TEntity> GetByIdAsync(Guid id, CancellationToken ct)
		=> GetByIdInternalAsync(id, Tracking, ct);

	public Task<TEntity?> GetByIdOrDefaultAsync(Guid id, CancellationToken ct)
		=> GetByIdOrDefaultInternalAsync(id, Tracking, ct);

	public Task<TEntity> GetByFilterAsync(
		Expression<Func<TEntity, bool>> predicate, 
		CancellationToken ct, 
		params string[] includes)
		=> GetByFilterInternalAsync(predicate, Tracking, ct, includes);

	public Task<TEntity?> GetByFilterOrDefaultAsync(
		Expression<Func<TEntity, bool>> predicate, 
		CancellationToken ct, 
		params string[] includes)
		=> GetByFilterOrDefaultInternalAsync(predicate, Tracking, ct, includes);

	public Task<List<TEntity>> GetManyByFilterAsync(
		Expression<Func<TEntity, bool>> predicate, 
		CancellationToken ct, 
		params string[] includes)
		=> GetManyByFilterInternalAsync(predicate, Tracking, ct, includes);

	public void Add(TEntity entity)
	{
		ArgumentNullException.ThrowIfNull(entity);
		Context.Set<TEntity>().Add(entity);
	}

	public void AddRange(params TEntity[] entities)
	{
		ArgumentNullException.ThrowIfNull(entities);

		if(entities.Length == 0)
		{
			return;
		}

		for(var i = 0; i < entities.Length; i++)
		{
			ArgumentNullException.ThrowIfNull(entities[i]);
		}

		Context.Set<TEntity>().AddRange(entities);
	}

	public void Remove(TEntity entity)
	{
		ArgumentNullException.ThrowIfNull(entity);
		Context.Set<TEntity>().Remove(entity);
	}

	public void RemoveRange(params TEntity[] entities)
	{
		ArgumentNullException.ThrowIfNull(entities);

		if(entities.Length == 0)
		{
			return;
		}

		for(var i = 0; i < entities.Length; i++)
		{
			ArgumentNullException.ThrowIfNull(entities[i]);
		}

		Context.Set<TEntity>().RemoveRange(entities);
	}

	public void Update(TEntity entity)
	{
		ArgumentNullException.ThrowIfNull(entity);
		Context.Set<TEntity>().Update(entity);
	}

	public void UpdateRange(params TEntity[] entities)
	{
		ArgumentNullException.ThrowIfNull(entities);

		if(entities.Length == 0)
		{
			return;
		}

		for(var i = 0; i < entities.Length; i++)
		{
			ArgumentNullException.ThrowIfNull(entities[i]);
		}

		Context.Set<TEntity>().UpdateRange(entities);
	}

}
