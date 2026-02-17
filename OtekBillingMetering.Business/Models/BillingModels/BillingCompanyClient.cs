using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.ValueObjects;

namespace OtekBillingMetering.Business.Models.BillingModels;

public sealed class BillingCompanyClient : Entity<Guid>
{
	private BillingCompanyClient() : base() { }

	private BillingCompanyClient(
		Guid id,
		string email,
		string displayName,
		string? phoneNumber,
		bool isEmailConfirmed = true,
		bool isPhoneNumberConfirmed = false) : base(id) =>
		UpdateContactInternal(
			email,
			displayName,
			phoneNumber,
			isEmailConfirmed,
			isPhoneNumberConfirmed);

	public string? DisplayName { get; private set; }
	public string Email { get; private set; } = null!;
	public bool EmailConfirmed { get; private set; } = false;
	public string? PhoneNumber { get; private set; } = string.Empty;
	public bool PhoneNumberConfirmed { get; private set; } = false;

	public Guid TenantId { get; private set; }

	public Guid? UserId { get; private set; }

	public Address? Address { get; private set; }

	internal static BillingCompanyClient CreateInternal(
		Guid id,
		string email,
		string displayName,
		string? phoneNumber,
		bool isEmailConfirmed = true,
		bool isPhoneNumberConfirmed = false) =>
		new(id, email, displayName, phoneNumber, isEmailConfirmed, isPhoneNumberConfirmed);

	internal void SetTenantInternal(Guid tenantId)
	{
		if(tenantId == Guid.Empty)
		{
			throw new DomainValidationException("TenantId is required.");
		}

		if(TenantId != Guid.Empty && TenantId != tenantId)
		{
			throw new DomainConflictException("TenantId cannot be changed.");
		}

		TenantId = tenantId;
	}

	internal void UpdateContactInternal(
		string email,
		string displayName,
		string? phoneNumber,
		bool isEmailConfirmed = true,
		bool isPhoneNumberConfirmed = false)
	{
		UpdateEmailInternal(email, isEmailConfirmed);
		UpdateDisplayNameInternal(displayName);
		UpdatePhoneNumberInternal(phoneNumber, isPhoneNumberConfirmed);
	}

	internal void SetAddressInternal(Address? address) => Address = address;

	internal void UpdateEmailInternal(string email, bool isConfirmed = true)
	{
		if(string.IsNullOrWhiteSpace(email))
		{
			throw new DomainValidationException("Email is required.");
		}

		Email = email.Trim();
		EmailConfirmed = isConfirmed;
	}

	internal void UpdatePhoneNumberInternal(string? phoneNumber, bool isConfirmed = false)
	{
		if(string.IsNullOrWhiteSpace(phoneNumber))
		{
			PhoneNumber = null;
			PhoneNumberConfirmed = false;
			return;
		}

		PhoneNumber = phoneNumber.Trim();
		PhoneNumberConfirmed = isConfirmed;
	}

	internal void UpdateDisplayNameInternal(string displayName) => DisplayName = string.IsNullOrWhiteSpace(displayName)
		? throw new DomainValidationException("DisplayName is required.")
		: displayName.Trim();

	internal void LinkToUserInternal(Guid userId)
	{
		if(userId == default)
		{
			throw new DomainValidationException("UserId cannot be empty.");
		}

		UserId = userId;
	}

	internal void UnlinkUserInternal() => UserId = null;
}
