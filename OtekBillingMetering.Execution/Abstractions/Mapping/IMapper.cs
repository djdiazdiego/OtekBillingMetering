namespace OtekBillingMetering.Execution.Abstractions.Mapping;

public interface IMapper
{
	TDestination Map<TSource, TDestination>(TSource source)
		where TSource : class
		where TDestination : class;
}
