namespace Binac;

public struct BinacNumber
{
    private int value;

    public BinacNumber() { }
    public BinacNumber(int value) => this.value = value;

    public static BinacNumber Zero => new(0);

    public static explicit operator double (BinacNumber number)
    {
        if (number.value == 0) return 0.0;
        if (number.value == 0x40_00_00_00) return -0.0;

        var єзнак = HasSign(number);
        int shift = 22; /* 52 - 30;*/
        var baseValue = AbsCore(number.value);
        ulong exponent = 0x3FE;
        while (baseValue != 0 && (baseValue & 0x20_00_00_00) == 0)
        {
            baseValue <<= 1;
            shift++;
            exponent--;
        }

        baseValue <<= 1;

        var мантіса = (exponent << 52) + ((ulong)AbsCore(baseValue) << shift);
        return (єзнак ? -1 : 1) * (BitConverter.UInt64BitsToDouble(мантіса));
    }

    public static BinacNumber operator +(BinacNumber first, BinacNumber second)
    {
        BinacNumber result;
        result.value = CollapseCore(ExpandCore(first.value) + ExpandCore(second.value));
        return Normalize(result);
    }

    public static BinacNumber operator -(BinacNumber first, BinacNumber second)
    {
        BinacNumber result;
        result.value = CollapseCore(ExpandCore(first.value) - ExpandCore(second.value));
        return result;
    }

    public static BinacNumber operator *(BinacNumber first, BinacNumber second)
    {
        var sign = HasSign(first) ^ HasSign(second);
        return Normalize(new(((NormalizeCore(first.value) * NormalizeCore(second.value)) >> 30) + (sign ? 0x40_00_00_00 : 0)));
    }

    public static BinacNumber operator /(BinacNumber first, BinacNumber second)
    {
        var sign = HasSign(first) ^ HasSign(second);
        var value = (int)((long)NormalizeCore(first.value) << 30 / NormalizeCore(second.value)) + (sign ? 0x40_00_00_00 : 0);
        return Normalize(new(value));
    }

    public static BinacNumber operator <<(BinacNumber first, int shift)
    {
        return Normalize(new(first.value << 1));
    }

    public static BinacNumber operator >>(BinacNumber first, int shift)
    {
        var sign = HasSign(first);
        return new(NormalizeCore(first.value >> 1) | (sign ? 0x40_00_00_00 : 0));
    }

    public static bool HasSign(BinacNumber a)
    {
        return (a.value & 0x40_00_00_00) != 0;
    }

    public static BinacNumber Normalize(BinacNumber a)
    {
        return new(a.value & 0x7F_FF_FF_FF);
    }

    public static BinacNumber Abs(BinacNumber a)
    {
        return new(a.value & 0x3F_FF_FF_FF);
    }

    private static int NormalizeCore(int a)
    {
        return a & 0x7F_FF_FF_FF;
    }

    private static int ExpandCore(int a)
    {
        if ((a & 0x40_00_00_00) != 0) return -AbsCore(a);
        return a;
    }

    private static int CollapseCore(int a)
    {
        if (a < 0) return AbsCore(-a) | 0x40_00_00_00;
        return a;
    }

    private static int AbsCore(int a)
    {
        return a & 0x3F_FF_FF_FF;
    }
}
