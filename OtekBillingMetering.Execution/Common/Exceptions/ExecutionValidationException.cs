namespace OtekBillingMetering.Execution.Common.Exceptions;

internal sealed class ExecutionValidationException : ExecutionException
{
	public ExecutionValidationException() : base() { }

	public ExecutionValidationException(string message) : base(message) { }

	public ExecutionValidationException(string message, Exception? innerException)
		: base(message, innerException) { }

	public ExecutionValidationException(string message, params object[] args)
		: base(message, args) { }

	// 400
	public override string Code => "execution_validation";
}
