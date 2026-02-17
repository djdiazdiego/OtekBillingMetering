// Ignore Spelling: Disconfirm

using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.ValueObjects;

namespace OtekBillingMetering.Business.Models.IdentityModels;

public sealed class User : Entity<Guid>
{
	private readonly List<Role> _roles = [];
	private readonly List<Group> _groups = [];
	private readonly List<Account> _managedAccounts = [];

	private User() : base() { }

	public User(string email, string? phoneNumber, string firstName, string? middleName, string lastName) =>
		Update(email, phoneNumber ?? string.Empty, firstName, middleName, lastName);

	public string UserName { get; private set; } = null!;
	public string Email { get; private set; } = null!;
	public bool EmailConfirmed { get; private set; } = false;
	public string? PhoneNumber { get; private set; } = string.Empty;
	public bool PhoneNumberConfirmed { get; private set; } = false;

	public string FirstName { get; private set; } = null!;
	public string? MiddleName { get; private set; } = string.Empty;
	public string LastName { get; private set; } = null!;

	public Guid? TenantId { get; private set; }

	public Address? Address { get; private set; }

	public IReadOnlyCollection<Role> Roles => _roles;
	public IReadOnlyCollection<Group> Groups => _groups;
	public IReadOnlyCollection<Account> ManagedAccounts => _managedAccounts;

	public void Update(string email, string phoneNumber, string firstName, string? middleName, string lastName)
	{
		UpdateEmail(email);
		UpdatePhoneNumber(phoneNumber ?? string.Empty);
		UpdateName(firstName, middleName, lastName);
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

	public void ClearAddress() => Address = null;

	public void ConfirmEmail() => EmailConfirmed = true;
	public void DisconfirmEmail() => EmailConfirmed = false;

	public void ConfirmPhoneNumber() => PhoneNumberConfirmed = true;
	public void DisconfirmPhoneNumber() => PhoneNumberConfirmed = false;

	public void UpdateName(string firstName, string? middleName, string lastName)
	{
		FirstName = string.IsNullOrWhiteSpace(firstName) ?
			throw new DomainValidationException("FirstName is required.") :
			firstName.Trim();
		LastName = string.IsNullOrWhiteSpace(lastName) ?
			throw new DomainValidationException("LastName is required.") :
			lastName.Trim();
		MiddleName = string.IsNullOrWhiteSpace(middleName) ? string.Empty : middleName.Trim();
	}

	public void UpdateEmail(string email)
	{
		Email = string.IsNullOrWhiteSpace(email) ?
			throw new DomainValidationException("Email is required.") :
			email.Trim();
		UserName = Email;
	}

	public void UpdatePhoneNumber(string phoneNumber) =>
		PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? string.Empty : phoneNumber.Trim();

	public void AddRoles(IEnumerable<Role> roles) => _roles.AddRange(roles.Where(r => !_roles.Contains(r)));

	public void RemoveRoles(IEnumerable<Role> roles)
	{
		foreach(var role in roles)
		{
			_roles.Remove(role);
		}
	}

	public void ClearRoles() => _roles.Clear();

	public void AddOwnAccounts(IEnumerable<Account> accounts)
	{
		var newAccounts = accounts.Where(a => !a.IsRoot && !_managedAccounts.Contains(a));
		_managedAccounts.AddRange(newAccounts);
	}

	public void RemoveOwnAccounts(IEnumerable<Account> accounts)
	{
		foreach(var account in accounts)
		{
			_managedAccounts.Remove(account);
		}
	}

	public void ClearOwnAccounts() => _managedAccounts.Clear();

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

		if(ManagedAccounts.Any(a => a.Id == tenantId))
		{
			throw new DomainConflictException("User cannot be assigned to an account they own.");
		}

		TenantId = tenantId;
	}
}
