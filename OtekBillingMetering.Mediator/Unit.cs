namespace OtekBillingMetering.Mediator;

internal readonly struct Unit : IEquatable<Unit>
{
	public static readonly Unit Value = new();

	public bool Equals(Unit other) => true;

	public override bool Equals(object? obj) => obj is Unit;

	public override int GetHashCode() => 0;

	public override string ToString() => "()";

	public static bool operator ==(Unit left, Unit right) => left.Equals(right);

	public static bool operator !=(Unit left, Unit right) => !(left == right);
}
