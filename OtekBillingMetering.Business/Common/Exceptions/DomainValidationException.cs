namespace OtekBillingMetering.Business.Common.Exceptions;

public sealed class DomainValidationException : DomainException
{
	public DomainValidationException() : base() { }

	public DomainValidationException(string message) : base(message) { }

	public DomainValidationException(string message, Exception? innerException)
		: base(message, innerException) { }

	public DomainValidationException(string message, params object[] args)
		: base(message, args) { }

	// 400
	public override string Code => "domain_validation";
}
