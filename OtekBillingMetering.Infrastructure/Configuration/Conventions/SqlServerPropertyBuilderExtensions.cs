using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OtekBillingMetering.Infrastructure.Configuration.Conventions;

internal static class SqlServerPropertyBuilderExtensions
{
	public static PropertyBuilder<Guid> UseSqlServerSequentialGuid(this PropertyBuilder<Guid> property)
			=> property.ValueGeneratedOnAdd()
				.HasDefaultValueSql(SqlServerDefaults.NEW_SEQUENTIAL_ID);
}
