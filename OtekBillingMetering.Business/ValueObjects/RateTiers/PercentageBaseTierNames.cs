using OtekBillingMetering.Business.Common.Exceptions;

namespace OtekBillingMetering.Business.ValueObjects.RateTiers;

public sealed record PercentageBaseTierNames
{
	public IReadOnlySet<string> TargetTierNames { get; }

	private PercentageBaseTierNames(IReadOnlySet<string> targetTierNames) =>
		TargetTierNames = targetTierNames;

	public static PercentageBaseTierNames Create(IEnumerable<string> targetTierNames, string ownerTierName)
	{
		var set = Normalize(targetTierNames);

		var normalizedOwnerTierName = string.IsNullOrWhiteSpace(ownerTierName)
			? throw new DomainValidationException("OwnerTierName is required.")
			: ownerTierName.Trim();

		return set.Contains(normalizedOwnerTierName)
			? throw new DomainValidationException("A percentage tier cannot target itself.")
			: new PercentageBaseTierNames(set);
	}

	public static PercentageBaseTierNames Restore(IEnumerable<string> targetTierNames)
	{
		var set = Normalize(targetTierNames);
		return new PercentageBaseTierNames(set);
	}

	private static HashSet<string> Normalize(IEnumerable<string> targetTierNames)
	{
		var set = targetTierNames?
			.Where(name => !string.IsNullOrWhiteSpace(name))
			.Select(name => name.Trim())
			.ToHashSet(StringComparer.Ordinal)
			?? throw new DomainValidationException("TargetTierNames are required.");

		return set.Count == 0
			? throw new DomainValidationException("At least one target tier is required.")
			: set;
	}
}