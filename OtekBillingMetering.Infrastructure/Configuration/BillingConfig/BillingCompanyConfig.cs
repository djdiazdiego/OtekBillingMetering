using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.BillingModels;
using OtekBillingMetering.Business.ValueObjects;
using OtekBillingMetering.Infrastructure.Configuration.Base;
using OtekBillingMetering.Infrastructure.Configuration.Conventions;

namespace OtekBillingMetering.Infrastructure.Configuration.BillingConfig;

internal sealed class BillingCompanyConfig : EntityConfigBase<BillingCompany>
{
	protected override void ConfigureEntity(EntityTypeBuilder<BillingCompany> builder)
	{
		builder.ToTable("BillingCompanies");

		builder.Property(x => x.TenantId)
			.IsRequired();

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(256)
			.IsUnicode(true);

		builder.Property(x => x.Description)
			.HasMaxLength(1024)
			.IsUnicode(true);

		builder.Property(x => x.IsActive)
			.IsRequired()
			.HasDefaultValue(true);

		builder.Property(x => x.BillingType)
			.IsRequired();

		builder.HasIndex(x => x.TenantId);
		builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();

		builder.OwnsOne(x => x.Address, owned =>
		{
			owned.Property(a => a.First)
				.HasColumnName($"{SqlServerDefaults.ADDRESS_PREFIX}{nameof(Address.First)}")
				.HasMaxLength(200)
				.IsUnicode(true)
				.IsRequired(false);

			owned.Property(a => a.Second)
				.HasColumnName($"{SqlServerDefaults.ADDRESS_PREFIX}{nameof(Address.Second)}")
				.HasMaxLength(200)
				.IsUnicode(true)
				.IsRequired(false);

			owned.Property(a => a.City)
				.HasColumnName($"{SqlServerDefaults.ADDRESS_PREFIX}{nameof(Address.City)}")
				.HasMaxLength(100)
				.IsUnicode(true)
				.IsRequired(false);

			owned.Property(a => a.State)
				.HasColumnName($"{SqlServerDefaults.ADDRESS_PREFIX}{nameof(Address.State)}")
				.HasMaxLength(100)
				.IsUnicode(true)
				.IsRequired(false);

			owned.Property(a => a.ZipCode)
				.HasColumnName($"{SqlServerDefaults.ADDRESS_PREFIX}{nameof(Address.ZipCode)}")
				.HasMaxLength(20)
				.IsUnicode(false)
				.IsRequired(false);

			owned.Property(a => a.Country)
				.HasColumnName($"{SqlServerDefaults.ADDRESS_PREFIX}{nameof(Address.Country)}")
				.HasMaxLength(100)
				.IsUnicode(true)
				.IsRequired(false);
		});

		builder.Navigation(x => x.Address).IsRequired(false);

		builder.Navigation(x => x.Clients).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.Navigation(x => x.ClientLinks).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.HasMany(x => x.Clients)
			.WithMany()
			.UsingEntity<BillingCompanyClientLink>(
				l => l.HasOne(x => x.Client)
					.WithMany()
					.HasForeignKey(x => x.ClientId)
					.OnDelete(DeleteBehavior.Cascade),
				l => l.HasOne(x => x.Company)
					.WithMany()
					.HasForeignKey(x => x.CompanyId)
					.OnDelete(DeleteBehavior.Cascade),
				j =>
				{
					j.ToTable("BillingCompanyClientLinks");

					j.HasKey(x => new { x.CompanyId, x.ClientId });

					j.Property(x => x.IsActive)
						.IsRequired()
						.HasDefaultValue(true);

					j.HasIndex(x => new { x.CompanyId, x.IsActive });
					j.HasIndex(x => new { x.ClientId, x.IsActive });
				});

		builder.HasMany(x => x.ClientLinks)
			.WithOne(x => x.Company)
			.HasForeignKey(x => x.CompanyId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
