using OtekBillingMetering.Business.Abstractions.BaseTypes;

namespace OtekBillingMetering.Execution.Abstractions.Persistence.Specifications;

public interface IPagedFilterSpec<TEntity> where TEntity : class, IEntity
{
	IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, Dictionary<string, string> searchText);
}