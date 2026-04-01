using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.BillingModels;
using OtekBillingMetering.Business.ValueObjects;
using OtekBillingMetering.Infrastructure.Configuration.Base;
using OtekBillingMetering.Infrastructure.Configuration.Conventions;

namespace OtekBillingMetering.Infrastructure.Configuration.BillingConfig;

internal sealed class BillingCompanyClientConfig : EntityConfigBase<BillingCompanyClient>
{
	protected override bool IsIdGeneratedByDatabase => false;

	protected override void ConfigureEntity(EntityTypeBuilder<BillingCompanyClient> builder)
	{
		builder.ToTable("BillingCompanyClients");

		builder.Property(x => x.TenantId)
			.IsRequired();

		builder.Property(x => x.Email)
			.IsRequired()
			.HasMaxLength(320)
			.IsUnicode(false);

		builder.Property(x => x.EmailConfirmed)
			.IsRequired()
			.HasDefaultValue(false);

		builder.Property(x => x.DisplayName)
			.IsRequired()
			.HasMaxLength(200)
			.IsUnicode(true);

		builder.Property(x => x.PhoneNumber)
			.HasMaxLength(32)
			.IsUnicode(false);

		builder.Property(x => x.PhoneNumberConfirmed)
			.IsRequired()
			.HasDefaultValue(false);

		builder.Property(x => x.UserId)
			.IsRequired(false);

		builder.HasIndex(x => x.TenantId);
		builder.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();

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
	}
}
