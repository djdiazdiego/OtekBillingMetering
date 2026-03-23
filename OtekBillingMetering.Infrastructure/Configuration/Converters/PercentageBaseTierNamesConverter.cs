// Ignore spelling: dto, json

using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OtekBillingMetering.Business.ValueObjects.RateTiers;

namespace OtekBillingMetering.Infrastructure.Configuration.Converters;

internal sealed class PercentageBaseTierNamesConverter
	: ValueConverter<PercentageBaseTierNames?, string>
{
	private sealed class Dto
	{
		public List<string> TargetTierNames { get; set; } = [];
	}

	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
	{
		PropertyNameCaseInsensitive = true
	};

	public PercentageBaseTierNamesConverter()
		: base(
			value => Serialize(value),
			json => Deserialize(json))
	{
	}

	private static string Serialize(PercentageBaseTierNames? value)
	{
		if(value is null)
		{
			return string.Empty;
		}

		var dto = new Dto
		{
			TargetTierNames = [.. value.TargetTierNames.OrderBy(x => x, StringComparer.Ordinal)]
		};

		return JsonSerializer.Serialize(dto, JsonOptions);
	}

	private static PercentageBaseTierNames? Deserialize(string json)
	{
		if(string.IsNullOrWhiteSpace(json))
		{
			return null;
		}

		var dto = JsonSerializer.Deserialize<Dto>(json, JsonOptions);

		return dto is null || dto.TargetTierNames.Count == 0 
			? null 
			: PercentageBaseTierNames.Restore(dto.TargetTierNames);
	}
}