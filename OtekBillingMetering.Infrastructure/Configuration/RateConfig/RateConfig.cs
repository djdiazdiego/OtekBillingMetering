using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Infrastructure.Configuration.Base;

namespace OtekBillingMetering.Infrastructure.Configuration.RateConfig;

internal sealed class RateConfig : EntityConfigBase<Rate>
{
	protected override string TableName => "Rates";

	protected override void ConfigureEntity(EntityTypeBuilder<Rate> builder)
	{
		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(x => x.Description)
			.IsRequired()
			.HasMaxLength(2000);

		builder.Property(x => x.UtilityId)
			.IsRequired();

		builder.Property(x => x.Utility)
			.IsRequired()
			.HasConversion<int>();

		builder.Property(x => x.TenantId)
			.IsRequired(false);

		builder.HasIndex(x => x.UtilityId);
		builder.HasIndex(x => x.TenantId);
		builder.HasIndex(x => new { x.TenantId, x.UtilityId });

		builder.HasMany(x => x.RateTiers)
			.WithOne(x => x.Rate)
			.HasForeignKey(x => x.RateId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(x => x.RateTiers)
			.UsePropertyAccessMode(PropertyAccessMode.Field);
	}
}