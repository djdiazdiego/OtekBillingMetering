using System.Runtime.Serialization;

namespace OtekBillingMetering.Business.Common.Types.Units;

public enum ValueUnitType
{
	[EnumMember(Value = "-")]
	None,
	[EnumMember(Value = "$")]
	Usd,
	[EnumMember(Value = "$/day")]
	UsdPerDay,
	[EnumMember(Value = "%")]
	Percent
}
