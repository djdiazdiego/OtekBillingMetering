using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.IdentityModels;
using OtekBillingMetering.Infrastructure.Configuration.Base;

namespace OtekBillingMetering.Infrastructure.Configuration.IdentityConfig;

internal sealed class PolicyConfig : EntityConfigBase<Policy>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Policy> builder)
	{
		builder.Property(x => x.Target)
			.IsRequired()
			.HasMaxLength(64)
			.IsUnicode(false);

		builder.Property(x => x.Action)
			.IsRequired()
			.HasConversion<int>();

		builder.Property(x => x.Description)
			.HasMaxLength(1024)
			.IsUnicode(true)
			.HasDefaultValue(string.Empty);

		builder.HasIndex(x => new { x.Target, x.Action })
			.IsUnique();

		builder.Navigation(x => x.Roles).Metadata
			.SetPropertyAccessMode(PropertyAccessMode.Field);
	}
}
