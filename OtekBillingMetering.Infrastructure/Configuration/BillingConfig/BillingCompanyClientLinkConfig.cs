using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.BillingModels;

namespace OtekBillingMetering.Infrastructure.Configuration.BillingConfig;

internal sealed class BillingCompanyClientLinkConfig : IEntityTypeConfiguration<BillingCompanyClientLink>
{
	public void Configure(EntityTypeBuilder<BillingCompanyClientLink> builder)
	{
		builder.ToTable("BillingCompanyClientLinks");

		builder.HasKey(x => new { x.CompanyId, x.ClientId });

		builder.Property(x => x.CompanyId).IsRequired();
		builder.Property(x => x.ClientId).IsRequired();

		builder.Property(x => x.IsActive)
			.IsRequired()
			.HasDefaultValue(true);

		builder.HasIndex(x => new { x.CompanyId, x.IsActive });
		builder.HasIndex(x => new { x.ClientId, x.IsActive });
	}
}
