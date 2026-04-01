using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.UtilityModels;
using OtekBillingMetering.Business.ValueObjects;
using OtekBillingMetering.Infrastructure.Configuration.Base;
using OtekBillingMetering.Infrastructure.Configuration.Conventions;

namespace OtekBillingMetering.Infrastructure.Configuration;

internal class UtilityConfig : EntityConfigBase<Utility>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Utility> builder){
		builder.ToTable("Utilities");

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(256)
			.IsUnicode(true);

		builder.Property(x => x.Type)
			.IsRequired();

		builder.Property(x => x.Description)
			.HasMaxLength(1024)
			.IsUnicode(true);

		builder.Property(x => x.TenantId)
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
	}
}
