// Ignore spelling: deserialize, json

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OtekBillingMetering.Infrastructure.Configuration.Extensions;

internal static class PropertyBuilderJsonExtensions
{
	public static PropertyBuilder<TProperty?> HasJsonValueObjectConversion<TProperty>(
		this PropertyBuilder<TProperty?> propertyBuilder,
		Func<TProperty?, string> serialize,
		Func<string, TProperty?> deserialize,
		Func<TProperty?, int> hashCode,
		Func<TProperty?, TProperty?> snapshot)
		where TProperty : class
	{
		var comparer = new ValueComparer<TProperty?>(
			(left, right) => Equals(left, right),
			value => hashCode(value),
			value => snapshot(value));

		propertyBuilder.HasConversion(
			value => serialize(value),
			value => deserialize(value));

		propertyBuilder.Metadata.SetValueComparer(comparer);

		return propertyBuilder;
	}
}