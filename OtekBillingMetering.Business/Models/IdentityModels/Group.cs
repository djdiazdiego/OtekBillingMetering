using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Business.Common.Exceptions;

namespace OtekBillingMetering.Business.Models.IdentityModels;

public sealed class Group : Entity<Guid>
{
	private readonly List<Role> _roles = [];
	private readonly List<User> _users = [];

	private Group() : base() { }

	public Group(string name, string? description = null) => Update(name, description);

	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }

	public Guid TenantId { get; private set; }

	public IReadOnlyCollection<Role> Roles => _roles;
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

	public void AddRoles(IEnumerable<Role> roles) => _roles.AddRange(roles.Where(r => !_roles.Contains(r)));

	public void RemoveRoles(IEnumerable<Role> roles)
	{
		foreach(var role in roles)
		{
			_roles.Remove(role);
		}
	}

	public void ClearRoles() => _roles.Clear();

	public void AddUsers(IEnumerable<User> users) => _users.AddRange(users.Where(u => !_users.Contains(u)));

	public void RemoveUsers(IEnumerable<User> users)
	{
		foreach(var user in users)
		{
			_users.Remove(user);
		}
	}

	public void ClearUsers() => _users.Clear();

	public void AddToAccount(Guid tenantId)
	{
		if(tenantId == default)
		{
			throw new DomainValidationException("TenantId cannot be empty.");
		}

		TenantId = tenantId;
	}
}