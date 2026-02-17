namespace OtekBillingMetering.Business.Common.Exceptions;

public sealed class DomainConflictException : DomainException
{
	public DomainConflictException() : base() { }

	public DomainConflictException(string message) : base(message) { }

	public DomainConflictException(string message, Exception? innerException)
		: base(message, innerException) { }

	public DomainConflictException(string message, params object[] args)
		: base(message, args) { }

	// 409
	public override string Code => "domain_conflict";
}
