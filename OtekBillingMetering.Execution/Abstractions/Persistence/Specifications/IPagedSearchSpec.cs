using OtekBillingMetering.Business.Abstractions;

namespace OtekBillingMetering.Execution.Abstractions.Persistence.Specifications;

public interface IPagedSearchSpec<TEntity> where TEntity : class, IEntity
{
	//Func<IQueryable<TEntity>, string, IQueryable<TEntity>> ApplySearch();
	IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string searchText);
}
