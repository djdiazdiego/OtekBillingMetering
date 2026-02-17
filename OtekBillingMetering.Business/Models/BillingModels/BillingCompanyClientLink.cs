using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Models.Billing;

namespace OtekBillingMetering.Business.Models.BillingModels;

public sealed class BillingCompanyClientLink
{
	private BillingCompanyClientLink() { }

	private BillingCompanyClientLink(Guid companyId, Guid clientId, bool isActive = true)
	{
		if(companyId == Guid.Empty)
		{
			throw new DomainValidationException("CompanyId is required.");
		}

		if(clientId == Guid.Empty)
		{
			throw new DomainValidationException("ClientId is required.");
		}

		CompanyId = companyId;
		ClientId = clientId;
		IsActive = isActive;
	}

	public Guid CompanyId { get; private set; }
	public Guid ClientId { get; private set; }

	public bool IsActive { get; private set; } = true;

	public BillingCompany Company { get; private set; } = null!;
	public BillingCompanyClient Client { get; private set; } = null!;

	internal static BillingCompanyClientLink CreateInternal(
		Guid companyId,
		Guid clientId,
		bool isActive = true) => new(companyId, clientId, isActive);

	internal void ActivateInternal() => IsActive = true;
	internal void DeactivateInternal() => IsActive = false;
}
