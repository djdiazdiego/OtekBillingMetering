using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Business.ValueObjects;
using OtekBillingMetering.Infrastructure.Configuration.Base;
using OtekBillingMetering.Infrastructure.Configuration.Conventions;

namespace OtekBillingMetering.Infrastructure.Configuration.IdentityConfig;

internal sealed class UserConfig : EntityConfigBase<User>
{
	protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");

		builder.Property(x => x.Email)
			.IsRequired()
			.HasMaxLength(320)
			.IsUnicode(false);

		builder.Property(x => x.UserName)
			.IsRequired()
			.HasMaxLength(256)
			.IsUnicode(false);

		builder.Property(x => x.EmailConfirmed)
			.IsRequired()
			.HasDefaultValue(false);

		builder.Property(x => x.PhoneNumber)
			.HasMaxLength(32)
			.IsUnicode(false);

		builder.Property(x => x.PhoneNumberConfirmed)
			.IsRequired()
			.HasDefaultValue(false);

		builder.Property(x => x.FirstName)
			.IsRequired()
			.HasMaxLength(100)
			.IsUnicode(true);

		builder.Property(x => x.MiddleName)
			.HasMaxLength(100)
			.IsUnicode(true);

		builder.Property(x => x.LastName)
			.IsRequired()
			.HasMaxLength(100)
			.IsUnicode(true);

		builder.Property(x => x.TenantId)
			.IsRequired(false);

		builder.HasIndex(x => x.TenantId);

		builder.HasIndex(x => x.Email).IsUnique();

		builder.HasIndex(x => x.UserName).IsUnique();

		builder.HasIndex(x => x.PhoneNumber);

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

		builder.Navigation(x => x.Roles).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.Navigation(x => x.Groups).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.Navigation(x => x.ManagedAccounts).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.HasMany(x => x.Roles)
			.WithMany(x => x.Users)
			.UsingEntity<Dictionary<string, object>>(
				"UserRoles",
				r => r.HasOne<Role>()
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade),
				l => l.HasOne<User>()
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade),
				j =>
				{
					j.ToTable("UserRoles");
					j.HasKey("UserId", "RoleId");
					j.HasIndex("RoleId", "UserId");
				});

		builder.HasMany(x => x.Groups)
			.WithMany(x => x.Users)
			.UsingEntity<Dictionary<string, object>>(
				"UserGroups",
				g => g.HasOne<Group>()
					.WithMany()
					.HasForeignKey("GroupId")
					.OnDelete(DeleteBehavior.Cascade),
				u => u.HasOne<User>()
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade),
				j =>
				{
					j.ToTable("UserGroups");
					j.HasKey("UserId", "GroupId");
					j.HasIndex("GroupId", "UserId");
				});

		builder.HasMany(x => x.ManagedAccounts)
			.WithMany()
			.UsingEntity<Dictionary<string, object>>(
				"UserManagedAccounts",
				a => a.HasOne<Account>()
					.WithMany()
					.HasForeignKey("AccountId")
					.OnDelete(DeleteBehavior.Restrict),
				u => u.HasOne<User>()
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade),
				j =>
				{
					j.ToTable("UserManagedAccounts");
					j.HasKey("UserId", "AccountId");
					j.HasIndex("AccountId", "UserId");
				});
	}
}
