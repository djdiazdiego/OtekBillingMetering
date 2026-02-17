using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OtekBillingMetering.Business.Abstractions.BaseTypes;
using OtekBillingMetering.Infrastructure.Configuration.Conventions;

namespace OtekBillingMetering.Infrastructure.Configuration.Base;

internal abstract class EntityConfigBase<TEntity> : IEntityTypeConfiguration<TEntity>
	where TEntity : Entity<Guid>
{
	public void Configure(EntityTypeBuilder<TEntity> builder)
	{
		ConfigureBase(builder);
		ConfigureEntity(builder);
	}

	protected virtual bool IsIdGeneratedByDatabase => true;

	protected virtual void ConfigureBase(EntityTypeBuilder<TEntity> builder)
	{
		builder.HasKey(x => x.Id);

		if(IsIdGeneratedByDatabase)
		{
			builder.Property(x => x.Id).UseSqlServerSequentialGuid();
		}
		else
		{
			builder.Property(x => x.Id).ValueGeneratedNever();
		}

		builder.Property(x => x.Version)
			.IsRowVersion()
			.IsConcurrencyToken();
	}

	protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
