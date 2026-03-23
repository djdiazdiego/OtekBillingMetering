//Ignore spelling: nvarchar

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Models.RateModels;
using OtekBillingMetering.Infrastructure.Configuration.Base;
using OtekBillingMetering.Infrastructure.Configuration.Comparers;
using OtekBillingMetering.Infrastructure.Configuration.Converters;

namespace OtekBillingMetering.Infrastructure.Configuration.RateConfig;

internal sealed class RateTierConfig : EntityConfigBase<RateTier>
{
	protected override string TableName => "RateTiers";

	protected override bool IsIdGeneratedByDatabase => false;

	protected override void ConfigureEntity(EntityTypeBuilder<RateTier> builder)
	{
		builder.Property(x => x.RateId)
			.IsRequired();

		builder.Property(x => x.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(x => x.RateTierType)
			.IsRequired()
			.HasConversion<int>();

		builder.Property(x => x.Multiplier)
			.IsRequired();

		builder.Property(x => x.UnitsPerCharge)
			.IsRequired(false);

		builder.Property(x => x.UnitType)
			.IsRequired()
			.HasConversion<int>();

		builder.Property(x => x.From)
			.IsRequired(false);

		builder.Property(x => x.To)
			.IsRequired(false);

		builder.Property(x => x.MonthFrom)
			.IsRequired(false)
			.HasConversion<int?>();

		builder.Property(x => x.MonthTo)
			.IsRequired(false)
			.HasConversion<int?>();

		builder.Property(x => x.DayOfMonthFrom)
			.IsRequired(false);

		builder.Property(x => x.DayOfMonthTo)
			.IsRequired(false);

		builder.Property(x => x.WeekdayFrom)
			.IsRequired(false)
			.HasConversion<int?>();

		builder.Property(x => x.WeekdayTo)
			.IsRequired(false)
			.HasConversion<int?>();

		builder.Property(x => x.TimeOfDayFrom)
			.HasColumnType("time")
			.IsRequired(false);

		builder.Property(x => x.TimeOfDayTo)
			.HasColumnType("time")
			.IsRequired(false);

		builder.Property(x => x.TenantId)
			.IsRequired(false);

		builder.Property(x => x.PercentageBase)
			.HasColumnName("PercentageBaseTierNames")
			.HasColumnType("nvarchar(max)")
			.HasConversion(new PercentageBaseTierNamesConverter())
			.Metadata.SetValueComparer(new PercentageBaseTierNamesComparer());

		builder.HasIndex(x => x.RateId);

		builder.HasIndex(x => x.TenantId);

		builder.HasIndex(x => new { x.RateId, x.Name })
			.IsUnique();

		builder.HasOne(x => x.Rate)
			.WithMany(x => x.RateTiers)
			.HasForeignKey(x => x.RateId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}