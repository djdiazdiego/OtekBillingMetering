using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Base;
using OtekBillingMetering.Execution.Abstractions.Persistence.Specifications;
using OtekBillingMetering.Execution.Abstractions.Wrappers;
using OtekBillingMetering.Execution.Common.Wrappers;
using OtekBillingMetering.Infrastructure.Persistence.DbContexts;
using OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;
using System.Linq.Expressions;

internal abstract class ReadRepository<TEntity>(ReadDbContext dbContext)
	: RepositoryBase<TEntity>(dbContext), IReadRepository<TEntity>
	where TEntity : class, IEntity
{
	private const bool Tracking = false;

	public Task<int> CountByFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
		=> CountByFilterInternalAsync(predicate, Tracking, ct);

	public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct, params string[] includes)
		=> ExistsInternalAsync(predicate, Tracking, ct, includes);

	public Task<TEntity> GetByFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct, params string[] includes)
		=> GetByFilterInternalAsync(predicate, Tracking, ct, includes);

	public Task<TEntity?> GetByFilterOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct, params string[] includes)
		=> GetByFilterOrDefaultInternalAsync(predicate, Tracking, ct, includes);

	public Task<TEntity> GetByIdAsync(Guid id, CancellationToken ct)
		=> GetByIdInternalAsync(id, Tracking, ct);

	public Task<TEntity?> GetByIdOrDefaultAsync(Guid id, CancellationToken ct)
		=> GetByIdOrDefaultInternalAsync(id, Tracking, ct);

	public Task<List<TEntity>> GetManyByFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct, params string[] includes)
		=> GetManyByFilterInternalAsync(predicate, Tracking, ct, includes);

	public Task<TProjection?> GetProjectionOrDefaultAsync<TProjection>(
		Expression<Func<TEntity, bool>>? predicate,
		Expression<Func<TEntity, TProjection>> selector,
		CancellationToken ct = default,
		params string[] includes)
		=> GetProjectionOrDefaultInternalAsync(predicate, selector, Tracking, ct, includes);

	public Task<List<TProjection>> GetProjectionsAsync<TProjection>(
		Expression<Func<TEntity, bool>>? predicate,
		Expression<Func<TEntity, TProjection>> selector,
		CancellationToken ct = default,
		params string[] includes)
		=> GetProjectionsInternalAsync(predicate, selector, Tracking, ct, includes);

	public Task<PagedResponse<TResult>> GetPaginatedAsync<TResult>(
		IPagedRequest request,
		Expression<Func<TEntity, TResult>> selector,
		IPagedSearchSpec<TEntity>? search = null,
		IPagedFilterSpec<TEntity>? filter = null,
		CancellationToken ct = default) where TResult : class
		=> GetPaginatedInternalAsync(request, selector, Tracking, search, filter, ct);
}
