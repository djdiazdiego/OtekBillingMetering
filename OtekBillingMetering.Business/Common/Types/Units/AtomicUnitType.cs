using System.Runtime.Serialization;

namespace OtekBillingMetering.Business.Common.Types.Units;

public enum AtomicUnitType
{
	// -------------------------
	// Volume (US)
	// -------------------------

	[EnumMember(Value = "ft³")]
	CubicFoot,

	[EnumMember(Value = "US gal")]
	UsGallon,

	[EnumMember(Value = "CCF")]
	Ccf,

	[EnumMember(Value = "MCF")]
	Mcf,

	[EnumMember(Value = "MG")]
	MillionUsGallons,

	[EnumMember(Value = "ac-ft")]
	AcreFoot,

	// -------------------------
	// Energy
	// -------------------------

	[EnumMember(Value = "Wh")]
	WattHour,

	[EnumMember(Value = "kWh")]
	KilowattHour,

	[EnumMember(Value = "MWh")]
	MegawattHour,

	[EnumMember(Value = "GWh")]
	GigawattHour,

	[EnumMember(Value = "MMBtu")]
	Mmbtu,

	[EnumMember(Value = "therm")]
	Therm,

	[EnumMember(Value = "Dth")]
	Dekatherm,

	// -------------------------
	// Power (Demand)
	// -------------------------

	[EnumMember(Value = "W")]
	Watt,

	[EnumMember(Value = "kW")]
	Kilowatt,

	[EnumMember(Value = "MW")]
	Megawatt,

	// -------------------------
	// Mass
	// -------------------------

	[EnumMember(Value = "lb")]
	Pound,

	[EnumMember(Value = "klb")]
	Kilopound
}
