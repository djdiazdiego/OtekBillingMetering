using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Common.Types.DateTime;
using OtekBillingMetering.Business.Common.Types.Units;
using OtekBillingMetering.Business.Models.UtilityModels.Types;
using OtekBillingMetering.Business.Policies.Billing;
using OtekBillingMetering.Business.Policies.TierValidation;
using OtekBillingMetering.Business.ValueObjects.RateTiers;

namespace OtekBillingMetering.Business.Models.RateModels;

public sealed class Rate : AggregateRoot<Guid>
{
	private readonly List<RateTier> _rateTiers = [];

	private Rate() : base() { }

	public Rate(string name, Guid utilityId, UtilityType utilityType, string? description) =>
		Update(name, utilityId, utilityType, description);

	public string Name { get; private set; } = null!;
	public string Description { get; private set; } = string.Empty;

	public UtilityType Utility { get; private set; }
	public Guid UtilityId { get; private set; }

	public Guid? TenantId { get; private set; }

	public IReadOnlyCollection<RateTier> RateTiers => _rateTiers;

	public void Update(string name, Guid utilityId, UtilityType utilityType, string? description)
	{
		UpdateName(name);
		UpdateUtility(utilityId, utilityType);
		UpdateDescription(description);
	}

	public void UpdateName(string name) =>
		Name = string.IsNullOrWhiteSpace(name)
			? throw new DomainValidationException("Name is required.")
			: NormalizeTierName(name);

	public void UpdateDescription(string? description) =>
		Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();

	public void UpdateUtility(Guid utilityId, UtilityType utilityType)
	{
		if(utilityId == Guid.Empty)
		{
			throw new DomainValidationException("Utility ID is required.");
		}

		UtilityId = utilityId;
		Utility = utilityType;

		if(_rateTiers.Count > 0)
		{
			ValidateTierRanges();
		}
	}

	public RateTier AddTier(RateTierDefinition definition)
	{
		EnsureUniqueTierNameOrThrow(definition.Name);

		var tier = new RateTier(Id, definition);

		if(tier.IsPercentageTier)
		{
			AssignPercentageBase(tier.Name, definition.PercentageTargetTierNames ?? []);
		}

		ValidateTierRanges();

		return tier;
	}

	public IReadOnlyList<RateTier> AddTiers(IEnumerable<RateTierDefinition> definitions)
	{
		var defs = definitions?.ToList() ?? throw new DomainValidationException("Tier definitions are required.");
		if(defs.Count == 0)
		{
			return [];
		}

		if(defs.Any(d => string.IsNullOrWhiteSpace(d.Name)))
		{
			throw new DomainValidationException("Tier definitions contain an empty Name.");
		}

		var dupInInput = defs
			.GroupBy(d => NormalizeTierName(d.Name), StringComparer.OrdinalIgnoreCase)
			.FirstOrDefault(g => g.Count() > 1);

		if(dupInInput is not null)
		{
			throw new DomainValidationException(
				"Tier definitions contain duplicate name '{0}'.",
				dupInInput.Key);
		}

		foreach(var d in defs)
		{
			EnsureUniqueTierNameOrThrow(d.Name);
		}

		var created = new List<RateTier>(defs.Count);
		foreach(var d in defs)
		{
			var tier = new RateTier(Id, d);
			_rateTiers.Add(tier);

			if(tier.IsPercentageTier)
			{
				AssignPercentageBase(tier.Name, d.PercentageTargetTierNames ?? []);
			}

			created.Add(tier);
		}

		ValidateTierRanges();

		return created;
	}

	public void UpdateTier(Guid tierId, RateTierDefinition definition)
	{
		var tier = GetTierOrThrow(tierId);

		EnsureUniqueTierNameOrThrow(definition.Name, excludeTierId: tierId);
		tier.Update(definition);

		if(tier.IsPercentageTier)
		{
			AssignPercentageBase(tier.Name, definition.PercentageTargetTierNames ?? []);
		}

		ValidateTierRanges();
	}

	public void RemoveTier(Guid tierId)
	{
		var tier = GetTierOrThrow(tierId);
		_rateTiers.Remove(tier);

		ValidateTierRanges();
	}

	public void ClearTiers() => _rateTiers.Clear();

	public void UpdateTierName(Guid tierId, string name)
	{
		var tier = GetTierOrThrow(tierId);
		EnsureUniqueTierNameOrThrow(name, excludeTierId: tierId);
		tier.ChangeName(name);
	}

	public void UpdateTierPricing(Guid tierId, double multiplier, double? unitsPerCharge, AtomicUnitType unitType)
	{
		var tier = GetTierOrThrow(tierId);
		tier.ChangePricing(multiplier, unitsPerCharge, unitType);
	}

	public void UpdateTierMultiplier(Guid tierId, double multiplier)
	{
		var tier = GetTierOrThrow(tierId);
		tier.ChangeMultiplier(multiplier);
	}

	public void UpdateTierUnitsPerCharge(Guid tierId, double? unitsPerCharge)
	{
		var tier = GetTierOrThrow(tierId);
		tier.ChangeUnitsPerCharge(unitsPerCharge);
	}

	public void UpdateTierMeasurementUnit(Guid tierId, AtomicUnitType unitType)
	{
		var tier = GetTierOrThrow(tierId);
		tier.ChangeMeasurementUnit(unitType);
	}

	public void UpdateTierRange(Guid tierId, double? from, double? to)
	{
		var tier = GetTierOrThrow(tierId);
		tier.ChangeRange(from, to);

		ValidateTierRanges();
	}

	public void UpdateTierTimeWindow(
		Guid tierId,
		MonthType? monthFrom, MonthType? monthTo,
		int? dayFrom, int? dayTo,
		WeekdayType? weekdayFrom, WeekdayType? weekdayTo,
		TimeOnly? timeFrom, TimeOnly? timeTo)
	{
		var tier = GetTierOrThrow(tierId);

		tier.ChangeTimeWindow(
			monthFrom, monthTo,
			dayFrom, dayTo,
			weekdayFrom, weekdayTo,
			timeFrom, timeTo
		);

		ValidateTierRanges();
	}

	public void AddToAccount(Guid tenantId)
	{
		if(TenantId.HasValue)
		{
			throw new DomainConflictException("Rate is already assigned to an account.");
		}

		if(tenantId == default)
		{
			throw new DomainValidationException("TenantId cannot be empty.");
		}

		TenantId = tenantId;

		foreach(var t in _rateTiers)
		{
			t.AddToAccount(tenantId);
		}
	}

	private void AssignPercentageBase(string percentageTierName, IEnumerable<string> targetTierNames)
	{
		if(string.IsNullOrWhiteSpace(percentageTierName))
		{
			throw new DomainValidationException("Percentage tier name is required.");
		}

		var tier = _rateTiers.SingleOrDefault(x => x.Name == percentageTierName)
			?? throw new DomainValidationException("Percentage tier not found.");

		var targets = targetTierNames?.ToList() ?? throw new DomainValidationException("Targets are required.");
		if(targets.Any(name => _rateTiers.All(t => t.Name != name)))
		{
			throw new DomainValidationException("One or more target tiers do not belong to this rate.");
		}

		tier.SetPercentageBase(targets);

		EnsureNoDependencyCycles();
	}

	private void EnsureNoDependencyCycles()
	{
		var adjacency = _rateTiers.ToDictionary(t => t.Name, _ => new List<string>());

		foreach(var t in _rateTiers)
		{
			var targets = t.PercentageBase?.TargetTierNames;
			if(targets is null || targets.Count == 0)
			{
				continue;
			}

			adjacency[t.Name].AddRange(targets);
		}

		var visited = new HashSet<string>();
		var stack = new HashSet<string>();

		foreach(var node in adjacency.Keys)
		{
			if(HasCycleDfs(node, adjacency, visited, stack))
			{
				throw new DomainValidationException("Circular dependency detected among rate tiers.");
			}
		}
	}

	private static bool HasCycleDfs(
		string node,
		IReadOnlyDictionary<string, List<string>> adjacency,
		HashSet<string> visited,
		HashSet<string> stack)
	{
		if(stack.Contains(node))
		{
			return true;
		}

		if(visited.Contains(node))
		{
			return false;
		}

		visited.Add(node);
		stack.Add(node);

		if(adjacency.TryGetValue(node, out var neighbors))
		{
			foreach(var next in neighbors)
			{
				if(!adjacency.ContainsKey(next))
				{
					continue;
				}

				if(HasCycleDfs(next, adjacency, visited, stack))
				{
					return true;
				}
			}
		}

		stack.Remove(node);
		return false;
	}

	private void ValidateTierRanges()
	{
		var billingPolicy = BillingPolicy.FromUtility(Utility);

		NonTimeRelatedRangeTiersPolicy.Validate(_rateTiers, billingPolicy);
		TimeRelatedRangeTiersPolicy.Validate(_rateTiers, billingPolicy);
	}

	private RateTier GetTierOrThrow(Guid tierId) =>
		_rateTiers.FirstOrDefault(x => x.Id == tierId)
		?? throw new DomainConflictException("RateTier not found.");

	private void EnsureUniqueTierNameOrThrow(string name, Guid? excludeTierId = null)
	{
		var normalized = NormalizeTierName(name);

		var exists = _rateTiers.Any(t =>
			(excludeTierId == null || t.Id != excludeTierId.Value) &&
			string.Equals(NormalizeTierName(t.Name), normalized, StringComparison.OrdinalIgnoreCase));

		if(exists)
		{
			throw new DomainConflictException("RateTier name '{0}' already exists in this Rate.", normalized);
		}
	}

	private static string NormalizeTierName(string name) => name.Trim();
}
