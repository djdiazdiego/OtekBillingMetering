namespace OtekBillingMetering.Business.ValueObjects;

public sealed record Address
{
	public string First { get; init; } = null!;
	public string? Second { get; init; }
	public string City { get; init; } = null!;
	public string State { get; init; } = null!;
	public string ZipCode { get; init; } = null!;
	public string Country { get; init; } = null!;

	private Address() { }

	public static Address Create(
		string first,
		string? second,
		string city,
		string state,
		string zipCode,
		string country) => new()
		{
			First = first.Trim(),
			Second = string.IsNullOrWhiteSpace(second) ? null : second.Trim(),
			City = city.Trim(),
			State = state.Trim(),
			ZipCode = zipCode.Trim(),
			Country = country.Trim(),
		};
}