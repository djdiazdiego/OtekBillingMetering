using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Infrastructure.Configuration.Base;

namespace OtekBillingMetering.Infrastructure.Configuration.IdentityConfig;

internal sealed class RoleConfig : EntityConfigBase<Role>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Role> builder)
	{
		builder.ToTable("Roles");

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(128)
			.IsUnicode(true);

		builder.Property(x => x.Description)
			.HasMaxLength(1024)
			.IsUnicode(true)
			.HasDefaultValue(string.Empty);

		builder.Property(x => x.TenantId)
			.IsRequired(true);

		builder.HasIndex(x => new { x.Name, x.TenantId })
			.IsUnique();	

		builder.Navigation(x => x.Policies).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.Navigation(x => x.Users).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.HasMany(x => x.Policies)
			.WithMany(x => x.Roles)
			.UsingEntity<Dictionary<string, object>>(
				"RolePolicies",
				p => p.HasOne<Policy>()
					.WithMany()
					.HasForeignKey("PolicyId")
					.OnDelete(DeleteBehavior.Cascade),
				r => r.HasOne<Role>()
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade),
				j =>
				{
					j.ToTable("RolePolicies");
					j.HasKey("RoleId", "PolicyId");
					j.HasIndex("PolicyId", "RoleId");
				});
	}
}
