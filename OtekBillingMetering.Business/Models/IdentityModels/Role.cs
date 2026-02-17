using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Business.Common.Exceptions;

namespace OtekBillingMetering.Business.Models.IdentityModels;

public sealed class Role : Entity<Guid>
{
	private readonly List<Policy> _policies = [];
	private readonly List<User> _users = [];

	private Role() : base() { }

	public Role(string name, string? description = null) => Update(name, description);

	public string Name { get; private set; } = null!;
	public string? Description { get; private set; } = string.Empty;

	public Guid TenantId { get; private set; }

	public IReadOnlyCollection<Policy> Policies => _policies;
	public IReadOnlyCollection<User> Users => _users;

	public void Update(string name, string? description)
	{
		UpdateName(name);
		UpdateDescription(description ?? string.Empty);
	}

	public void UpdateName(string name) =>
		Name = string.IsNullOrWhiteSpace(name) ? throw new DomainValidationException("Name is required.") : name.Trim();

	public void UpdateDescription(string description) =>
		Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();

	public void AddPolicies(IEnumerable<Policy> policies) => _policies.AddRange(policies.Where(p => !_policies.Contains(p)));

	public void RemovePolicies(IEnumerable<Policy> policies)
	{
		foreach(var policy in policies)
		{
			_policies.Remove(policy);
		}
	}

	public void ClearPolicies() => _policies.Clear();

	public void AddToAccount(Guid tenantId)
	{
		if(tenantId == default)
		{
			throw new DomainValidationException("TenantId cannot be empty.");
		}

		TenantId = tenantId;
	}
}
