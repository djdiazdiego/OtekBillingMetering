using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Infrastructure.Configuration.Base;

namespace OtekBillingMetering.Infrastructure.Configuration.IdentityConfig;

internal sealed class GroupConfig : EntityConfigBase<Group>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Group> builder)
	{
		builder.ToTable("Groups");

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

		builder.Navigation(x => x.Roles).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.Navigation(x => x.Users).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.HasMany(x => x.Roles)
			.WithMany()
			.UsingEntity<Dictionary<string, object>>(
				"GroupRoles",
				r => r.HasOne<Role>()
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade),
				g => g.HasOne<Group>()
					.WithMany()
					.HasForeignKey("GroupId")
					.OnDelete(DeleteBehavior.Cascade),
				j =>
				{
					j.ToTable("GroupRoles");
					j.HasKey("GroupId", "RoleId");
					j.HasIndex("RoleId", "GroupId");
				});
	}
}
