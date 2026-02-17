using System.Runtime.Serialization;

namespace OtekBillingMetering.Business.Common.Types.DateTime;

public enum MonthType
{
	[EnumMember(Value = "Jan")]
	January,
	[EnumMember(Value = "Feb")]
	February,
	[EnumMember(Value = "Mar")]
	March,
	[EnumMember(Value = "Apr")]
	April,
	[EnumMember(Value = "May")]
	May,
	[EnumMember(Value = "Jun")]
	June,
	[EnumMember(Value = "Jul")]
	July,
	[EnumMember(Value = "Aug")]
	August,
	[EnumMember(Value = "Sep")]
	September,
	[EnumMember(Value = "Oct")]
	October,
	[EnumMember(Value = "Nov")]
	November,
	[EnumMember(Value = "Dec")]
	December
}
