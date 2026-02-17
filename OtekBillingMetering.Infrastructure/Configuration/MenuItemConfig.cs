using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.MenuItemModels;
using OtekBillingMetering.Infrastructure.Configuration.Base;

namespace OtekBillingMetering.Infrastructure.Configuration;

internal sealed class MenuItemConfig : EntityConfigBase<MenuItem>
{
	protected override void ConfigureEntity(EntityTypeBuilder<MenuItem> builder)
	{
		builder.ToTable("MenuItems");

		builder.Property(x => x.Key)
			.IsRequired()
			.HasMaxLength(256)
			.IsUnicode(false);

		builder.HasIndex(x => x.Key)
			.IsUnique();

		builder.Property(x => x.Area)
			.IsRequired();

		builder.Property(x => x.Type)
			.IsRequired();

		builder.Property(x => x.TitleKey)
			.IsRequired()
			.HasMaxLength(256)
			.IsUnicode(false);

		builder.Property(x => x.TitleFallback)
			.HasMaxLength(256)
			.IsUnicode(true);

		builder.Property(x => x.Route)
			.HasMaxLength(512)
			.IsUnicode(false);

		builder.Property(x => x.ExternalUrl)
			.HasMaxLength(1024)
			.IsUnicode(false);

		builder.Property(x => x.Icon)
			.HasMaxLength(64)
			.IsUnicode(false);

		builder.Property(x => x.SortOrder)
			.IsRequired();

		builder.Property(x => x.IsVisible)
			.IsRequired()
			.HasDefaultValue(true);

		builder.Property(x => x.IsEnabled)
			.IsRequired()
			.HasDefaultValue(true);

		builder.Navigation(x => x.Children).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);

		builder.HasOne(x => x.Parent)
			.WithMany(x => x.Children)
			.HasForeignKey(x => x.ParentId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(x => new { x.Area, x.ParentId, x.SortOrder });
	}
}
