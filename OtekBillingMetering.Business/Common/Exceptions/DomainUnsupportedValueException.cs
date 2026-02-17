namespace OtekBillingMetering.Business.Common.Exceptions;

public sealed class DomainUnsupportedValueException : DomainException
{
	public DomainUnsupportedValueException() : base() { }

	public DomainUnsupportedValueException(string message) : base(message) { }

	public DomainUnsupportedValueException(string message, Exception? innerException)
		: base(message, innerException) { }

	public DomainUnsupportedValueException(string message, params object[] args)
		: base(message, args) { }

	// 500
	public override string Code => "domain_unsupported_value";
}