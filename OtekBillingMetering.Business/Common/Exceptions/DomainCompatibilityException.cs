namespace OtekBillingMetering.Business.Common.Exceptions;

public sealed class DomainCompatibilityException : DomainException
{
	public DomainCompatibilityException() : base() { }

	public DomainCompatibilityException(string message) : base(message) { }

	public DomainCompatibilityException(string message, Exception? innerException)
		: base(message, innerException) { }

	public DomainCompatibilityException(string message, params object[] args)
		: base(message, args) { }

	// 500
	public override string Code => "domain_compatibility";
}