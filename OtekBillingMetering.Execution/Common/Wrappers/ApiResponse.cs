namespace OtekBillingMetering.Execution.Common.Wrappers;

public record ApiResponse
{
	public bool Success { get; init; } = true;
	public string? Message { get; init; }
	public int Status { get; set; }
	public IReadOnlyList<ApiError>? Errors { get; init; } = null;

	public static ApiResponse Ok(string? message = null, int status = 200)
		=> new() { Success = true, Message = message, Status = status };

	public static ApiResponse Fail(
		string? message = "BadRequestException", 
		int status = 400, 
		IReadOnlyList<ApiError>? errors = null)
			=> new()
			{
				Success = false,
				Message = message,
				Status = status,
				Errors = errors,
			};
}

public record ApiResponse<T> : ApiResponse
{
	public T? Data { get; init; }

	public static ApiResponse<T> Ok(T data, string? message = null, int status = 200)
		=> new()
		{
			Success = true,
			Data = data,
			Message = message,
			Status = status
		};
}

public record ApiError
{
	public string Target { get; init; } = "error";
	public string Message { get; init; } = "An error occurred.";
}

