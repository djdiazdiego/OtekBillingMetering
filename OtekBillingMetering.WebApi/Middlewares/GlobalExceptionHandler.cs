using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Execution.Common.Wrappers;

namespace OtekBillingMetering.WebApi.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext context,
		Exception exception,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(exception);

		var statusCode = MapStatusCode(exception);
		var publicMessage = MapPublicMessage(statusCode);

		LogException(context, exception, statusCode);

		context.Response.StatusCode = statusCode;

		var response = ApiResponse.Fail(
			message: publicMessage,
			errors: [],
			status: statusCode);

		await context.Response.WriteAsJsonAsync(
			response,
			cancellationToken: cancellationToken);

		return true;
	}

	private void LogException(HttpContext context, Exception exception, int statusCode)
	{
		var path = context.Request.Path;
		var traceId = context.TraceIdentifier;
		var time = DateTime.UtcNow;

		if(exception is ValidationException validationException)
		{
			var details = BuildValidationDetails(validationException);

			logger.LogWarning(
				exception,
				"Validation error. Path: {Path}, TraceId: {TraceId}, Time: {Time}, Details: {Details}",
				path,
				traceId,
				time,
				details);

			return;
		}

		logger.LogError(
			exception,
			"Unhandled exception. StatusCode: {StatusCode}, Path: {Path}, TraceId: {TraceId}, Time: {Time}",
			statusCode,
			path,
			traceId,
			time);
	}

	private static string BuildValidationDetails(ValidationException validationException)
	{
		var details = new StringBuilder();

		foreach(var error in validationException.Errors)
		{
			details.AppendLine($"{error.PropertyName}: {error.ErrorMessage}");
		}

		return details.ToString().TrimEnd();
	}

	private static int MapStatusCode(Exception exception) => exception switch
	{
		DomainValidationException => StatusCodes.Status400BadRequest,
		ValidationException => StatusCodes.Status400BadRequest,
		KeyNotFoundException => StatusCodes.Status404NotFound,
		DomainConflictException => StatusCodes.Status409Conflict,
		UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
		DomainUnsupportedValueException => StatusCodes.Status500InternalServerError,
		DomainCompatibilityException => StatusCodes.Status500InternalServerError,
		_ => StatusCodes.Status500InternalServerError
	};

	private static string MapPublicMessage(int statusCode) => statusCode switch
	{
		StatusCodes.Status400BadRequest => "Bad Request",
		StatusCodes.Status401Unauthorized => "Unauthorized",
		StatusCodes.Status403Forbidden => "Forbidden",
		StatusCodes.Status404NotFound => "Not Found",
		StatusCodes.Status409Conflict => "Conflict",
		_ => "Internal Server Error"
	};
}