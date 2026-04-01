using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Business.ValueObjects;
using OtekBillingMetering.Infrastructure.Configuration.Base;
using OtekBillingMetering.Infrastructure.Configuration.Conventions;

namespace OtekBillingMetering.Infrastructure.Configuration.IdentityConfig;

internal sealed class AccountConfig : EntityConfigBase<Account>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Account> builder)
	{
		builder.ToTable("Accounts");

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(128)
			.IsUnicode(true);

		builder.Property(x => x.Description)
			.HasMaxLength(1024)
			.IsUnicode(true);

		builder.Property(x => x.IsActive)
			.IsRequired()
			.HasDefaultValue(true);

		builder.Property(x => x.IsRoot)
			.IsRequired()
			.HasDefaultValue(false);

		builder.Property(x => x.TenantId)
			.IsRequired(false);

		builder.HasOne(x => x.Tenant)
			.WithMany()
			.HasForeignKey(x => x.TenantId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasIndex(x => new { x.TenantId, x.Name })
			.IsUnique()
			.HasFilter("[TenantId] IS NOT NULL");

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
