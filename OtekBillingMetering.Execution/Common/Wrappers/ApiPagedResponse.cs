namespace OtekBillingMetering.Execution.Common.Wrappers;

public record PagedResponse<T> : ApiResponse<IReadOnlyList<T>>
{
	public PageMeta Page { get; init; } = null!;

	public static PagedResponse<T> Ok(
		IReadOnlyList<T> items,
		PageMeta page,
		string? message = null,
		int status = 200)
		=> new()
		{
			Success = true,
			Data = items,
			Page = page,
			Message = message,
			Status = status
		};
}

public record PageMeta
{
	public int PageNumber { get; init; }
	public int PageSize { get; init; }
	public long TotalRecords { get; init; }
	public int TotalPages { get; init; }

	public bool HasPrevious => TotalPages > 0 && PageNumber > 1;
	public bool HasNext => TotalPages > 0 && PageNumber < TotalPages;

	public static PageMeta BuildPageMeta(long totalRecords, int pageNumber, int pageSize)
	{
		if(pageSize <= 0)
		{
			pageSize = 25;
		}

		if(pageNumber <= 0)
		{
			pageNumber = 1;
		}

		var totalPages = ComputeTotalPages(totalRecords, pageSize);

		if(totalPages > 0 && pageNumber > totalPages)
		{
			pageNumber = totalPages;
		}

		return new PageMeta
		{
			PageNumber = pageNumber,
			PageSize = pageSize,
			TotalRecords = totalRecords,
			TotalPages = totalPages
		};
	}

	private static int ComputeTotalPages(long totalRecords, int pageSize) =>
		pageSize <= 0 || totalRecords <= 0 ? 0 : (int)((totalRecords + pageSize - 1) / pageSize);
}