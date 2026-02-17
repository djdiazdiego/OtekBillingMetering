using System.Globalization;

namespace OtekBillingMetering.Execution.Common.Exceptions;

internal abstract class ExecutionException : Exception
{
	protected ExecutionException() : base() { }

	protected ExecutionException(string message) : base(message) { }

	protected ExecutionException(string message, Exception? innerException)
		: base(message, innerException) { }

	protected ExecutionException(string message, params object[] args)
		: base(string.Format(CultureInfo.CurrentCulture, message, args)) { }

	public abstract string Code { get; }
}
