using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.ValueObjects;

namespace OtekBillingMetering.Business.Models.IdentityModels;

public sealed class Account : Entity<Guid>
{
	private Account() : base() { }

	public Account(
		string name,
		string? description = null,
		bool isRoot = false,
		bool isActive = true)
	{
		Update(name, description, isActive);
		IsRoot = isRoot;
	}

	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }

	public bool IsActive { get; private set; } = true;
	public bool IsRoot { get; private set; } = false;

	public Address? Address { get; private set; }

	public Guid? TenantId { get; private set; }

	public Account? Tenant { get; private set; }

	public void Update(
		string name,
		string? description,
		bool isActive = true)
	{
		UpdateName(name);
		UpdateDescription(description ?? string.Empty);

		IsActive = isActive;
	}

	public void SetAddress(Address? address) => Address = address;

	public void SetAddress(
	string addressFirst,
	string? addressSecond,
	string city,
	string state,
	string zipCode,
	string country) => Address = Address.Create(
		first: addressFirst,
		second: addressSecond,
		city: city,
		state: state,
		zipCode: zipCode,
		country: country);

	public void UpdateName(string name) => Name = string.IsNullOrWhiteSpace(name) ?
		throw new DomainValidationException("Name is required.") : name.Trim();

	public void UpdateDescription(string description) => 
		Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

	public void AssignToTenant(Guid tenantId)
	{
		if(TenantId.HasValue)
		{
			throw new DomainConflictException("User is already assigned to an account.");
		}

		if(tenantId == Guid.Empty)
		{
			throw new DomainValidationException("TenantId cannot be empty.");
		}

		if(Id == tenantId)
		{
			throw new DomainConflictException("A tenant cannot be assigned to itself.");
		}

		TenantId = tenantId;
	}

	public void Activate() => IsActive = true;
	public void Deactivate() => IsActive = false;
}
