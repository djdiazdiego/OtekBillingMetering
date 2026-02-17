using System.Reflection;
using System.Runtime.Serialization;

namespace OtekBillingMetering.Business.Common.Extensions;

public static class EnumMemberExtensions
{
	public static string GetEnumMemberValue<TEnum>(this TEnum value)
		where TEnum : struct, Enum
	{
		var type = typeof(TEnum);
		var name = Enum.GetName(type, value);

		if(name is null)
		{
			return value.ToString();
		}

		var field = type.GetField(name, BindingFlags.Public | BindingFlags.Static);
		if(field is null)
		{
			return name;
		}

		var attr = field.GetCustomAttributes(typeof(EnumMemberAttribute), inherit: false)
						.Cast<EnumMemberAttribute>()
						.FirstOrDefault();

		return string.IsNullOrWhiteSpace(attr?.Value) ? name : attr!.Value!;
	}

	public static bool TryParseEnumMemberValue<TEnum>(string? enumMemberValue, out TEnum result)
		where TEnum : struct, Enum
	{
		result = default;

		if(string.IsNullOrWhiteSpace(enumMemberValue))
		{
			return false;
		}

		var type = typeof(TEnum);

		foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
		{
			var attr = field.GetCustomAttributes(typeof(EnumMemberAttribute), inherit: false)
				.Cast<EnumMemberAttribute>()
				.FirstOrDefault();

			if(!string.IsNullOrWhiteSpace(attr?.Value) &&
				string.Equals(attr!.Value, enumMemberValue, StringComparison.OrdinalIgnoreCase))
			{
				result = (TEnum)field.GetValue(null)!;
				return true;
			}
		}

		return false;
	}
}
