using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Models.BillingModels;
using OtekBillingMetering.Business.Models.BillingModels.Types;
using OtekBillingMetering.Business.ValueObjects;

namespace OtekBillingMetering.Business.Models.Billing;

public sealed class BillingCompany : AggregateRoot<Guid>
{
	private readonly List<BillingCompanyClient> _clients = [];
	private readonly List<BillingCompanyClientLink> _clientLinks = [];

	private BillingCompany() : base() { }

	public BillingCompany(
		Guid tenantId,
		string name,
		CompanyBillingType billingType,
		string? description = null)
	{
		SetOwnerAccount(tenantId);
		Update(name, description);
		SetBillingType(billingType);
	}

	public Guid TenantId { get; private set; }

	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }

	public bool IsActive { get; private set; } = true;

	public CompanyBillingType BillingType { get; private set; }

	public Address? Address { get; private set; }

	public IReadOnlyCollection<BillingCompanyClient> Clients => _clients;
	public IReadOnlyCollection<BillingCompanyClientLink> ClientLinks => _clientLinks;

	public BillingCompanyClient CreateAndAttachClient(
		string email,
		string displayName,
		string? phoneNumber,
		bool isEmailConfirmed = true,
		bool isPhoneNumberConfirmed = false)
	{
		var id = Guid.CreateVersion7();

		var client = BillingCompanyClient.CreateInternal(
			id,
			email,
			displayName,
			phoneNumber,
			isEmailConfirmed,
			isPhoneNumberConfirmed);

		client.SetTenantInternal(TenantId);

		AttachClient(client);
		return client;
	}

	public void Update(string name, string? description)
	{
		Name = string.IsNullOrWhiteSpace(name)
			? throw new DomainValidationException("Company name is required.")
			: name.Trim();

		Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
	}

	public void SetOwnerAccount(Guid tenantId)
	{
		if(tenantId == Guid.Empty)
		{
			throw new DomainValidationException("TenantId is required.");
		}

		TenantId = tenantId;
	}

	public void SetAddress(Address? address) => Address = address;

	public void SetBillingType(CompanyBillingType billingType) => BillingType = billingType;

	public void Activate() => IsActive = true;
	public void Deactivate() => IsActive = false;

	public void AttachClient(BillingCompanyClient client)
	{
		if(client is null)
		{
			throw new DomainValidationException("Client is required.");
		}

		if(client.Id == Guid.Empty)
		{
			throw new DomainValidationException("ClientId is required.");
		}

		if(client.TenantId != TenantId)
		{
			throw new DomainConflictException("Client must belong to the same tenant as the company.");
		}

		if(_clientLinks.Any(x => x.ClientId == client.Id))
		{
			throw new DomainConflictException("Client is already attached to this company.");
		}

		_clientLinks.Add(BillingCompanyClientLink.CreateInternal(Id, client.Id, isActive: true));

		if(!_clients.Contains(client))
		{
			_clients.Add(client);
		}
	}

	public void DetachClient(Guid clientId)
	{
		if(clientId == Guid.Empty)
		{
			return;
		}

		var link = _clientLinks.FirstOrDefault(x => x.ClientId == clientId);
		if(link is not null)
		{
			_clientLinks.Remove(link);
		}

		var client = _clients.FirstOrDefault(x => x.Id == clientId);
		if(client is not null)
		{
			_clients.Remove(client);
		}
	}

	public void ClearClients()
	{
		_clients.Clear();
		_clientLinks.Clear();
	}

	public void UpdateClientContact(
		Guid clientId,
		string email,
		string displayName,
		string? phoneNumber,
		bool isEmailConfirmed = true,
		bool isPhoneNumberConfirmed = false)
	{
		if(clientId == Guid.Empty)
		{
			throw new DomainValidationException("ClientId is required.");
		}

		var client = _clients.FirstOrDefault(x => x.Id == clientId) ??
			throw new DomainValidationException("Client not found.");

		if(client.TenantId != TenantId)
		{
			throw new DomainConflictException("Client must belong to the same tenant as the company.");
		}

		client.UpdateContactInternal(email, displayName, phoneNumber, isEmailConfirmed, isPhoneNumberConfirmed);
	}

	public void LinkClientToUser(BillingCompanyClient client, Guid userId)
	{
		if(client is null)
		{
			throw new DomainValidationException("Client is required.");
		}

		if(client.TenantId != TenantId)
		{
			throw new DomainConflictException("Client must belong to the same tenant as the company.");
		}

		client.LinkToUserInternal(userId);
	}

	public void UnlinkClientToUser(BillingCompanyClient client)
	{
		if(client is null)
		{
			throw new DomainValidationException("Client is required.");
		}

		if(client.TenantId != TenantId)
		{
			throw new DomainConflictException("Client must belong to the same tenant as the company.");
		}

		client.UnlinkUserInternal();
	}

	public void DeactivateClient(Guid clientId)
	{
		if(clientId == Guid.Empty)
		{
			throw new DomainValidationException("ClientId is required.");
		}

		var link = _clientLinks.FirstOrDefault(x => x.ClientId == clientId)
			?? throw new DomainConflictException("Client is not attached to this company.");

		link.DeactivateInternal();
	}

	public void ActivateClient(Guid clientId)
	{
		if(clientId == Guid.Empty)
		{
			throw new DomainValidationException("ClientId is required.");
		}

		var link = _clientLinks.FirstOrDefault(x => x.ClientId == clientId)
			?? throw new DomainConflictException("Client is not attached to this company.");

		link.ActivateInternal();
	}
}
