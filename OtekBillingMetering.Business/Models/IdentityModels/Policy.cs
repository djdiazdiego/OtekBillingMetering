using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Models.IdentityModels.Types;

namespace OtekBillingMetering.Business.Models.IdentityModels;

public sealed class Policy : Entity<Guid>
{
	private readonly List<Role> _roles = [];

	private Policy() : base() { }

	public Policy(string target, PolicyActionType action, string? description = null) =>
		Update(target, action, description);

	public string Target { get; private set; } = null!;
	public PolicyActionType Action { get; private set; }
	public string? Description { get; private set; } = string.Empty;

	public IReadOnlyCollection<Role> Roles => _roles;

	public void Update(string target, PolicyActionType action, string? description)
	{
		UpdateTarget(target);
		UpdateAction(action);
		UpdateDescription(description ?? string.Empty);
	}

	public void UpdateTarget(string target) => Target = string.IsNullOrWhiteSpace(target)
		? throw new DomainValidationException("Target is required.")
		: target.Trim();

	public void UpdateAction(PolicyActionType action) => Action = action;

	public void UpdateDescription(string description) =>
		Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
}
