using FluentValidation;
using OtekBillingMetering.Mediator.Abstractions;

namespace OtekBillingMetering.Execution.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>(
	IEnumerable<IValidator<TRequest>> validators)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest
	where TResponse : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var validatorArray = validators as IValidator<TRequest>[] ?? [.. validators];

		if(validatorArray.Length == 0)
		{
			return await next(cancellationToken).ConfigureAwait(false);
		}

		var context = new ValidationContext<TRequest>(request);

		var results = await Task.WhenAll(
			validatorArray.Select(v => v.ValidateAsync(context, cancellationToken)))
			.ConfigureAwait(false);

		var failures = results
			.SelectMany(r => r.Errors)
			.Where(f => f is not null)
			.ToList();

		return failures.Count > 0 ? throw new ValidationException(failures) :
			await next(cancellationToken).ConfigureAwait(false);
	}
}
