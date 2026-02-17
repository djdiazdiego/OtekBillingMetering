namespace OtekBillingMetering.Execution.Common.Security;

public static class PermissionTargets
{
	public static class Identity
	{
		public const string Accounts = "identity.accounts";
		public const string Users = "identity.users";
		public const string Roles = "identity.roles";
		public const string Groups = "identity.groups";
		public const string Policies = "identity.policies";
	}

	public static class Utility
	{
		public const string Providers = "utility.providers";
		public const string Services = "utility.services";
	}

	public static class Billing
	{
		public const string Statements = "billing.statements";
		public const string Run = "billing.run";
		public const string Invoices = "billing.invoices";
		public const string Payments = "billing.payments";
	}

	public static class Metering
	{
		public const string Meters = "metering.meters";
		public const string Readings = "metering.readings";
		public const string Rates = "metering.rates";
	}

	public static class Rate
	{
		public const string Plans = "rate.plans";
		public const string Tiers = "rate.tiers";
	}

	public static class System
	{
		public const string Settings = "system.settings";
		public const string Logs = "system.logs";
	}
}
