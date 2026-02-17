using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Execution.Abstractions.Persistence.Specifications;
using OtekBillingMetering.Execution.Abstractions.Wrappers;
using OtekBillingMetering.Execution.Common.Wrappers;
using System.Linq.Expressions;

namespace OtekBillingMetering.Execution.Abstractions.Persistence.Repositories.Base;

public interface IReadRepository<TEntity> where TEntity : class, IEntity
{
	Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);

	Task<TEntity?> GetByIdOrDefaultAsync(Guid id, CancellationToken cancellationToken);

	Task<bool> ExistsAsync(
		Expression<Func<TEntity, bool>> predicate, 
		CancellationToken cancellationToken,
		params string[] includes);

	Task<TEntity> GetByFilterAsync(
		Expression<Func<TEntity, bool>> predicate, 
		CancellationToken cancellationToken,
		params string[] includes);

	Task<TEntity?> GetByFilterOrDefaultAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken cancellationToken, 
		params string[] includes);

	Task<List<TEntity>> GetManyByFilterAsync(
		Expression<Func<TEntity, bool>> predicate,
		CancellationToken cancellationToken, 
		params string[] includes);

	Task<TProjection?> GetProjectionOrDefaultAsync<TProjection>(
		Expression<Func<TEntity, bool>>? predicate,
		Expression<Func<TEntity, TProjection>> selector,
		CancellationToken cancellationToken = default,
		params string[] includes);

	Task<List<TProjection>> GetProjectionsAsync<TProjection>(
		Expression<Func<TEntity, bool>>? predicate,
		Expression<Func<TEntity, TProjection>> selector,
		CancellationToken cancellationToken = default,
		params string[] includes);

	Task<int> CountByFilterAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

	Task<PagedResponse<TResult>> GetPaginatedAsync<TResult>(
		IPagedRequest request,
		Expression<Func<TEntity, TResult>> selector,
		IPagedSearchSpec<TEntity>? search = null,
		IPagedFilterSpec<TEntity>? filter = null,
		CancellationToken cancellationToken = default
	) where TResult : class;
}
