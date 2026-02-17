using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Models.UtilityModels.Types;
using OtekBillingMetering.Business.ValueObjects;

namespace OtekBillingMetering.Business.Models.UtilityModels;

public sealed class Utility : Entity<Guid>
{
	private Utility() : base() { }

	public Utility(string name, UtilityType type, Address address, string? description) =>
		Update(name, type, address, description);

	public string Name { get; private set; } = null!;
	public UtilityType Type { get; private set; }
	public string Description { get; private set; } = string.Empty;

	public Guid? TenantId { get; private set; }

	public Address Address { get; private set; } = null!;

	public void Update(string name, UtilityType type, Address address, string? description)
	{
		UpdateName(name);
		UpdateType(type);
		UpdateAddress(address);
		UpdateDescription(description);
	}

	public void UpdateType(UtilityType type) => Type = type;

	public void UpdateName(string name) => Name = string.IsNullOrWhiteSpace(name) ?
		throw new DomainValidationException("Name is required.") : name.Trim();

	public void UpdateDescription(string? description) => Description = string.IsNullOrWhiteSpace(description) ?
		string.Empty : description.Trim();

	public void UpdateAddress(Address address) => Address = address ?? throw new DomainValidationException("Address is required.");

	public void AddToAccount(Guid tenantId)
	{
		if(TenantId.HasValue)
		{
			throw new DomainConflictException("Utility is already assigned to an account.");
		}

		if(tenantId == default)
		{
			throw new DomainValidationException("TenantId cannot be empty.");
		}

		TenantId = tenantId;
	}
}
