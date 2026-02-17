using System.Runtime.Serialization;

namespace OtekBillingMetering.Business.Common.Types.DateTime;

public enum WeekdayType
{
	[EnumMember(Value = "Mon")]
	Monday,
	[EnumMember(Value = "Tue")]
	Tuesday,
	[EnumMember(Value = "Wed")]
	Wednesday,
	[EnumMember(Value = "Thu")]
	Thursday,
	[EnumMember(Value = "Fri")]
	Friday,
	[EnumMember(Value = "Sat")]
	Saturday,
	[EnumMember(Value = "Sun")]
	Sunday

}
