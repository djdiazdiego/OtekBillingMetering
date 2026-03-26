using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OtekBillingMetering.Execution;
using OtekBillingMetering.Execution.Common.Wrappers;
using OtekBillingMetering.Infrastructure;
using OtekBillingMetering.WebApi.Configuration;
using OtekBillingMetering.WebApi.Middlewares;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

const string WEB_CLIENT_CORS_POLICY = "WebClientCorsPolicy";
const string RATE_LIMITING_POLICY = "fixed";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExecution(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options
	=> options.AddPolicy(WEB_CLIENT_CORS_POLICY, policy =>
		{
			policy.WithHeaders("Content-Type", "Authorization")
					.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
					.AllowCredentials();

			if(builder.Environment.IsDevelopment())
			{
				policy.WithOrigins("http://localhost:4200");
			}
			else
			{
				policy.WithOrigins("https://production.com", "https://*.production.com")
					.SetIsOriginAllowedToAllowWildcardSubdomains();
			}
		}));

builder.Services.AddRouting(x => x.LowercaseUrls = true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context =>
	{
		var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger("ModelValidation");

		var details = new StringBuilder();

		foreach(var kvp in context.ModelState)
		{
			if(kvp.Value is { Errors.Count: > 0 })
			{
				var error = string.Join(". ", kvp.Value.Errors.Select(e => e.ErrorMessage.Trim(' ', '.')));
				details.AppendLine($"{kvp.Key}: {error}");
			}
		}

		var statusCode = 400;

		logger.LogError(
			"Validation error. StatusCode: {StatusCode}, Path: {Path}, TraceId: {TraceId}, Time: {Time}, Details: {Details}",
			statusCode,
			context.HttpContext.Request.Path,
			context.HttpContext.TraceIdentifier,
			DateTime.UtcNow,
			details.ToString());

		var apiError = new ApiError
		{
			Target = "request",
			Message = "Invalid request data. Please check the provided information."
		};

		return new BadRequestObjectResult(ApiResponse.Fail(errors: [apiError]));
	});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHsts(options =>
{
	options.MaxAge = TimeSpan.FromDays(365);
	options.IncludeSubDomains = true;
	options.Preload = true;
});

builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.AddFixedWindowLimiter(RATE_LIMITING_POLICY, limiterOptions =>
	{
		limiterOptions.Window = TimeSpan.FromMinutes(1);
		limiterOptions.PermitLimit = 100;
	});
});

//builder.Services.AddEndpointsApiExplorer(); USE WITH SWAGGER

var app = builder.Build();

app.UseExceptionHandler();

if(app.Environment.IsDevelopment())
{

}
else
{
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(WEB_CLIENT_CORS_POLICY);

app.UseRateLimiter();

app.MapAppHealthChecks();
app.MapControllers().RequireRateLimiting(RATE_LIMITING_POLICY);

app.Run();
