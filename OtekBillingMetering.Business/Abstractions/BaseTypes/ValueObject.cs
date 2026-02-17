namespace OtekBillingMetering.Business.Abstractions.BaseTypes;

public abstract class ValueObject
{
	protected static bool EqualOperator(ValueObject left, ValueObject right) =>
		!(left is null ^ right is null) && (left is null || left.Equals(right));

	protected static bool NotEqualOperator(ValueObject left, ValueObject right) =>
		!EqualOperator(left, right);

	protected abstract IEnumerable<object> GetEqualityComponents();

	public override bool Equals(object? obj) =>
		obj != null && obj is ValueObject valueObject && obj.GetType() == GetType() &&
		GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());

	public override int GetHashCode() => GetEqualityComponents()
		.Select(x => x != null ? x.GetHashCode() : 0)
		.Aggregate((x, y) => x ^ y);

	public ValueObject? GetCopy() => MemberwiseClone() as ValueObject;
}

