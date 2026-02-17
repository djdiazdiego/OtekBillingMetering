using System.Text.Json.Serialization;
using OtekBillingMetering.Execution.Abstractions.Wrappers;
using OtekBillingMetering.Execution.Common.Wrappers.Types;

namespace OtekBillingMetering.Execution.Common.Wrappers;

public sealed class ApiPagedRequest : IPagedRequest
{
	public const int DefaultPageNumber = 1;
	public const int DefaultPageSize = 25;
	public const int MaxPageSize = 100;

	private int _pageNumber = DefaultPageNumber;
	private int _pageSize = DefaultPageSize;

	public int PageNumber
	{
		get => _pageNumber;
		set => _pageNumber = value < 1 ? DefaultPageNumber : value;
	}

	public int PageSize
	{
		get => _pageSize;
		set => _pageSize = value switch
		{
			< 1 => 1,
			> MaxPageSize => MaxPageSize,
			_ => value
		};
	}

	public string? Search { get; init; }

	public string? SortBy { get; init; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public SortDirection SortDirection { get; init; } = SortDirection.Asc;

	public Dictionary<string, string>? Filters { get; init; }

	public List<string>? IncludeNavigationProperties { get; init; }

	[JsonIgnore]
	public int Skip => (PageNumber - 1) * PageSize;

	[JsonIgnore]
	public int Take => PageSize;

	public PageMeta ToPageMeta(long totalRecords)
		=> PageMeta.BuildPageMeta(totalRecords, PageNumber, PageSize);

	public ApiPagedRequest Normalize()
		=> new()
		{
			PageNumber = PageNumber,
			PageSize = PageSize,
			Search = Search,
			SortBy = SortBy,
			SortDirection = SortDirection,
			Filters = Filters,
			IncludeNavigationProperties = IncludeNavigationProperties
		};
}
