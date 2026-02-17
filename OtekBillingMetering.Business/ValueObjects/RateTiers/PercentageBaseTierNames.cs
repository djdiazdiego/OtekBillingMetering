using OtekBillingMetering.Business.Common.Exceptions;

namespace OtekBillingMetering.Business.ValueObjects.RateTiers;

public sealed record PercentageBaseTierNames
{
	public IReadOnlySet<string> TargetTierNames { get; }

	private PercentageBaseTierNames(IReadOnlySet<string> targetTierNames) =>
		TargetTierNames = targetTierNames;

	public static PercentageBaseTierNames Create(IEnumerable<string> targetTierNames, string ownerTierName)
	{
		var set = targetTierNames?
			.Where(name => !string.IsNullOrWhiteSpace(name))
			.Select(name => name.Trim())
			.ToHashSet() ?? throw new DomainValidationException("TargetTierNames are required.");

		return set.Count == 0
			? throw new DomainValidationException("At least one target tier is required.")
			: set.Contains(ownerTierName)
			? throw new DomainValidationException("A percentage tier cannot target itself.")
			: new PercentageBaseTierNames(set);
	}
}

