using System.Globalization;

namespace OtekBillingMetering.Business.Common.Exceptions;

public abstract class DomainException : Exception
{
	protected DomainException() : base() { }

	protected DomainException(string message) : base(message) { }

	protected DomainException(string message, Exception? innerException)
		: base(message, innerException) { }

	protected DomainException(string message, params object[] args)
		: base(string.Format(CultureInfo.CurrentCulture, message, args)) { }

	public abstract string Code { get; }
}
