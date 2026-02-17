using Microsoft.EntityFrameworkCore;
using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Execution.Abstractions.Persistence.Specifications;
using OtekBillingMetering.Execution.Abstractions.Wrappers;
using OtekBillingMetering.Execution.Common.Wrappers;
using OtekBillingMetering.Execution.Common.Wrappers.Types;
using System.Linq.Expressions;

namespace OtekBillingMetering.Infrastructure.Persistence.Repositories.Base;

internal abstract class RepositoryBase<TEntity>(DbContext dbContext)
	where TEntity : class, IEntity
{
	protected readonly DbContext Context = dbContext;

	protected IQueryable<TEntity> Query(bool tracking)
	{
		var set = Context.Set<TEntity>().AsQueryable();
		return tracking ? set : set.AsNoTracking();
	}

	protected static IQueryable<TEntity> ApplyIncludes(
		IQueryable<TEntity> query, 
		params string[] includes)
	{
		if(includes is null || includes.Length == 0)
		{
			return query;
		}

		foreach(var include in includes)
		{
			if(!string.IsNullOrWhiteSpace(include))
			{
				query = query.Include(include);
			}
		}

		return query;
	}

	protected static string NormalizePropertyName(string? sortBy)
	{
		if(string.IsNullOrWhiteSpace(sortBy))
		{
			return nameof(IEntity.Id);
		}

		var prop = typeof(TEntity).GetProperties()
			.FirstOrDefault(p => string.Equals(p.Name, sortBy, StringComparison.OrdinalIgnoreCase) 
				&& p.CanRead);

		return prop?.Name ?? nameof(IEntity.Id);
	}

	protected static IQueryable<TEntity> ApplyOrdering(
		IQueryable<TEntity> query,
		string? sortBy,
		SortDirection direction)
	{
		var normalizedSortBy = NormalizePropertyName(sortBy);

		var parameter = Expression.Parameter(typeof(TEntity), "x");
		var property = Expression.PropertyOrField(parameter, normalizedSortBy);

		var delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), property.Type);
		var lambda = Expression.Lambda(delegateType, property, parameter);

		var methodName = direction == SortDirection.Asc
			? nameof(Queryable.OrderBy)
			: nameof(Queryable.OrderByDescending);

		var method = typeof(Queryable).GetMethods()
			.Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
			.Where(m => m.GetParameters().Length == 2)
			.Single();

		var generic = method.MakeGenericMethod(typeof(TEntity), property.Type);

		return (IQueryable<TEntity>)generic.Invoke(null, [query, lambda])!;
	}

	protected async Task<TEntity> GetByIdInternalAsync(Guid id, bool tracking, CancellationToken ct)
		=> await Query(tracking)
			.FirstAsync(x => x.Id.Equals(id), ct)
			.ConfigureAwait(false);

	protected async Task<TEntity?> GetByIdOrDefaultInternalAsync(
		Guid id, 
		bool tracking, 
		CancellationToken ct)
		=> await Query(tracking)
			.FirstOrDefaultAsync(x => x.Id.Equals(id), ct)
			.ConfigureAwait(false);

	protected async Task<TEntity> GetByFilterInternalAsync(
		Expression<Func<TEntity, bool>> predicate,
		bool tracking,
		CancellationToken ct,
		params string[] includes)
	{
		var q = ApplyIncludes(Query(tracking), includes);
		return await q.FirstAsync(predicate, ct).ConfigureAwait(false);
	}

	protected async Task<TEntity?> GetByFilterOrDefaultInternalAsync(
		Expression<Func<TEntity, bool>> predicate,
		bool tracking,
		CancellationToken ct,
		params string[] includes)
	{
		var q = ApplyIncludes(Query(tracking), includes);
		return await q.FirstOrDefaultAsync(predicate, ct).ConfigureAwait(false);
	}

	protected async Task<List<TEntity>> GetManyByFilterInternalAsync(
		Expression<Func<TEntity, bool>> predicate,
		bool tracking,
		CancellationToken ct,
		params string[] includes)
	{
		var q = ApplyIncludes(Query(tracking), includes);
		return await q.Where(predicate).ToListAsync(ct).ConfigureAwait(false);
	}

	protected async Task<bool> ExistsInternalAsync(
		Expression<Func<TEntity, bool>> predicate,
		bool tracking,
		CancellationToken ct,
		params string[] includes)
	{
		var q = ApplyIncludes(Query(tracking), includes);
		return await q.AnyAsync(predicate, ct).ConfigureAwait(false);
	}

	protected async Task<int> CountByFilterInternalAsync(
		Expression<Func<TEntity, bool>> predicate,
		bool tracking,
		CancellationToken ct)
		=> await Query(tracking).CountAsync(predicate, ct).ConfigureAwait(false);

	protected async Task<TProjection?> GetProjectionOrDefaultInternalAsync<TProjection>(
		Expression<Func<TEntity, bool>>? predicate,
		Expression<Func<TEntity, TProjection>> selector,
		bool tracking,
		CancellationToken ct,
		params string[] includes)
	{
		var q = ApplyIncludes(Query(tracking), includes);

		if(predicate != null)
		{
			q = q.Where(predicate);
		}

		return await q.Select(selector).FirstOrDefaultAsync(ct).ConfigureAwait(false);
	}

	protected async Task<List<TProjection>> GetProjectionsInternalAsync<TProjection>(
		Expression<Func<TEntity, bool>>? predicate,
		Expression<Func<TEntity, TProjection>> selector,
		bool tracking,
		CancellationToken ct,
		params string[] includes)
	{
		var q = ApplyIncludes(Query(tracking), includes);

		if(predicate != null)
		{
			q = q.Where(predicate);
		}

		return await q.Select(selector).ToListAsync(ct).ConfigureAwait(false);
	}

	protected async Task<PagedResponse<TResult>> GetPaginatedInternalAsync<TResult>(
		IPagedRequest request,
		Expression<Func<TEntity, TResult>> selector,
		bool tracking,
		IPagedSearchSpec<TEntity>? search,
		IPagedFilterSpec<TEntity>? filter,
		CancellationToken ct) where TResult : class
	{
		var q = Query(tracking);

		if(request.IncludeNavigationProperties != null)
		{
			foreach(var include in request.IncludeNavigationProperties)
			{
				q = q.Include(include);
			}
		}

		if(search != null && !string.IsNullOrWhiteSpace(request.Search))
		{
			q = search.ApplySearch(q, request.Search);
		}

		if(request.Filters != null && request.Filters.Count != 0 && filter is not null)
		{
			q = filter.ApplyFilter(q, request.Filters);
		}

		q = ApplyOrdering(q, request.SortBy, request.SortDirection);

		var totalRecords = await q.CountAsync(ct).ConfigureAwait(false);

		var items = await q
			.Skip(request.Skip)
			.Take(request.Take)
			.Select(selector)
			.ToListAsync(ct)
			.ConfigureAwait(false);

		var pageMeta = PageMeta.BuildPageMeta(totalRecords, request.PageNumber, request.PageSize);

		return PagedResponse<TResult>.Ok(items, pageMeta, "Paginated result successfully retrieved.");
	}
}
