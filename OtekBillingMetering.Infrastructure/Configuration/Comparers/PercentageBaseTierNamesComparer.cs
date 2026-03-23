using Microsoft.EntityFrameworkCore.ChangeTracking;
using OtekBillingMetering.Business.ValueObjects.RateTiers;

namespace OtekBillingMetering.Infrastructure.Configuration.Comparers;

internal sealed class PercentageBaseTierNamesComparer
	: ValueComparer<PercentageBaseTierNames?>
{
	public PercentageBaseTierNamesComparer()
		: base(
			(left, right) => AreEqual(left, right),
			value => ComputeHash(value),
			value => CreateSnapshot(value))
	{
	}

	private static bool AreEqual(
		PercentageBaseTierNames? left,
		PercentageBaseTierNames? right)
		=> ReferenceEquals(left, right)
			|| (left is not null
			&& right is not null
			&& left.TargetTierNames.SetEquals(right.TargetTierNames));

	private static int ComputeHash(PercentageBaseTierNames? value)
	{
		if(value is null)
		{
			return 0;
		}

		var hash = new HashCode();

		foreach(var item in value.TargetTierNames.OrderBy(x => x, StringComparer.Ordinal))
		{
			hash.Add(item, StringComparer.Ordinal);
		}

		return hash.ToHashCode();
	}

	private static PercentageBaseTierNames? CreateSnapshot(PercentageBaseTierNames? value)
		=> value is null
			? null
			: PercentageBaseTierNames.Restore(value.TargetTierNames);
}