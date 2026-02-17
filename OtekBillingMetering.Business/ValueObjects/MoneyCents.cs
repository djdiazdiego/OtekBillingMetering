// Ignore Spelling: nano

using System.Globalization;
using OtekBillingMetering.Business.Common.Types;

namespace OtekBillingMetering.Business.ValueObjects;

public readonly record struct MoneyCents : IComparable<MoneyCents>
{
	private const long Scale = 1_000_000_000L;
	private const long NanoPerCent = Scale / 100L;

	public const long NanoPerDollar = Scale;
	public const long NanoPerCentConst = NanoPerCent;

	private readonly long _nanoDollars;

	private MoneyCents(long nanoDollars) => _nanoDollars = nanoDollars;

	public static MoneyCents Zero { get; } = new(0);

	public long NanoDollars => _nanoDollars;

	public bool IsZero => _nanoDollars == 0;
	public bool IsPositive => _nanoDollars > 0;
	public bool IsNegative => _nanoDollars < 0;

	public MoneyCents Abs() => new(Math.Abs(_nanoDollars));

	public static MoneyCents FromNanoDollars(long nanoDollars) => new(nanoDollars);

	public static MoneyCents FromCents(long cents)
	{
		checked
		{
			return new MoneyCents(cents * NanoPerCent);
		}
	}

	public static MoneyCents FromDollars(decimal dollars)
	{
		var nano = dollars * Scale;

		if(nano is > long.MaxValue or < long.MinValue)
		{
			throw new OverflowException($"Dollar-to-nano overflowed. dollars={dollars}");
		}

		var rounded = decimal.ToInt64(decimal.Round(nano, 0, MidpointRounding.AwayFromZero));
		return new MoneyCents(rounded);
	}

	public decimal ToMoney()
		=> (decimal)_nanoDollars / Scale;

	public long ToCents(RoundingModeType mode = RoundingModeType.HalfEven)
		=> RoundDiv(_nanoDollars, NanoPerCent, mode);

	public MoneyCents RoundedToCents(RoundingModeType mode = RoundingModeType.HalfEven)
		=> FromCents(ToCents(mode));

	public MoneyCents Times(decimal factor)
	{
		var v = _nanoDollars * factor;

		if(v is > long.MaxValue or < long.MinValue)
		{
			throw new OverflowException($"Money multiply overflowed. nano={_nanoDollars}, factor={factor}");
		}

		var rounded = decimal.ToInt64(decimal.Round(v, 0, MidpointRounding.AwayFromZero));
		return new MoneyCents(rounded);
	}

	public MoneyCents DifferenceAbs(MoneyCents other)
	{
		checked
		{
			var diff = _nanoDollars - other._nanoDollars;
			return new MoneyCents(Math.Abs(diff));
		}
	}

	public int CompareTo(MoneyCents other)
		=> _nanoDollars.CompareTo(other._nanoDollars);

	public bool IsGreaterThan(MoneyCents other) => CompareTo(other) > 0;
	public bool IsGreaterThanOrEqual(MoneyCents other) => CompareTo(other) >= 0;

	public bool IsLessThan(MoneyCents other) => CompareTo(other) < 0;
	public bool IsLessThanOrEqual(MoneyCents other) => CompareTo(other) <= 0;

	public bool IsGreaterThanAbs(MoneyCents other) => Abs().CompareTo(other.Abs()) > 0;
	public bool IsGreaterThanOrEqualAbs(MoneyCents other) => Abs().CompareTo(other.Abs()) >= 0;

	public bool IsLessThanAbs(MoneyCents other) => Abs().CompareTo(other.Abs()) < 0;
	public bool IsLessThanOrEqualAbs(MoneyCents other) => Abs().CompareTo(other.Abs()) <= 0;

	public static MoneyCents operator +(MoneyCents a, MoneyCents b)
	{
		checked
		{
			return new MoneyCents(a._nanoDollars + b._nanoDollars);
		}
	}

	public static MoneyCents operator -(MoneyCents a, MoneyCents b)
	{
		checked
		{
			return new MoneyCents(a._nanoDollars - b._nanoDollars);
		}
	}

	public override string ToString()
	{
		var cents = ToCents(RoundingModeType.HalfEven);

		var neg = cents < 0;
		var abs = Math.Abs(cents);

		var dollars = abs / 100;
		var rem = abs % 100;

		var two = rem < 10 ? $"0{rem}" : rem.ToString(CultureInfo.InvariantCulture);
		return neg ? $"-{dollars}.{two}" : $"{dollars}.{two}";
	}

	private static long RoundDiv(long n, long d, RoundingModeType mode)
	{
		if(d <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(d), $"denominator must be > 0. Got {d}");
		}

		var sign = n < 0 ? -1 : 1;
		var an = n == long.MinValue ? (ulong)long.MaxValue + 1UL : (ulong)Math.Abs(n);

		var q = (long)(an / (ulong)d);
		var r = (long)(an - (ulong)q * (ulong)d);

		return r == 0 ? sign * q : mode switch
		{
			RoundingModeType.Down => sign < 0 ? -(q + 1) : q,
			RoundingModeType.Up => sign < 0 ? -q : q + 1,
			RoundingModeType.Nearest => sign * (q + (r * 2 >= d ? 1 : 0)),
			RoundingModeType.HalfEven => sign * HalfEven(q, r, d),

			_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
		};

		static long HalfEven(long q, long r, long d)
		{
			var twiceR = r * 2;
			return twiceR < d ? q : twiceR > d ? q + 1 : (q % 2 == 0) ? q : q + 1;
		}
	}
}
