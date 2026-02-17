using Microsoft.EntityFrameworkCore;
using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Execution.Abstractions.Persistence.Specifications;

namespace OtekBillingMetering.Infrastructure.Persistence.Specifications;

internal sealed class AccountSearchSpec : IPagedSearchSpec<Account>
{
	public static readonly AccountSearchSpec Instance = new();

	private AccountSearchSpec() { }

	public IQueryable<Account> ApplySearch(IQueryable<Account> query, string searchText)
	{
		if(string.IsNullOrWhiteSpace(searchText))
		{
			return query;
		}

		var pattern = $"%{searchText.Trim()}%";
		return query.Where(a =>
			EF.Functions.Like(
				EF.Functions.Collate(a.Name, PersistenceConstants.CI_AI_COLLATION),
				pattern));
	}
}

internal sealed class AccountFilterSpec : IPagedFilterSpec<Account>
{
	public static readonly AccountFilterSpec Instance = new();

	private AccountFilterSpec() { }

	public IQueryable<Account> ApplyFilter(IQueryable<Account> query, Dictionary<string, string> filters)
	{
		if(filters is null || filters.Count == 0)
		{
			return query;
		}

		if(filters.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
		{
			var pattern = $"%{name.Trim()}%";
			query = query.Where(a =>
				EF.Functions.Like(
					EF.Functions.Collate(a.Name, PersistenceConstants.CI_AI_COLLATION),
					pattern));
		}

		return query;
	}
}