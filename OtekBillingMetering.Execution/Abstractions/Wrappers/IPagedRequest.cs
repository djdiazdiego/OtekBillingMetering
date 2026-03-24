using OtekBillingMetering.Execution.Common.Wrappers.Types;

namespace OtekBillingMetering.Execution.Abstractions.Wrappers;

public interface IPagedRequest
{
	int PageNumber { get; }
	int PageSize { get; }
	string? Search { get; }
	string? SortBy { get; }
	SortDirection SortDirection { get; }

	Dictionary<string, string>? Filters { get; }
	List<string>? IncludeNavigationProperties { get; }

	int Skip { get; }
	int Take { get; }
}