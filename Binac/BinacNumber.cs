namespace Binac;

public struct BinacNumber
{
    private int value;

    public BinacNumber() { }
    public BinacNumber(int value) => this.value = value;

    public static BinacNumber Zero => new(0);

    public static explicit operator double (BinacNumber number)
    {
        var єзнак = HasSign(number);
        var мантіса = (0x3FF0UL << 49) + ((ulong)NormalizeCore(number.value) << (53 - 30));
        return (єзнак ? -1 : 1) * (BitConverter.UInt64BitsToDouble(мантіса) - 1.0);
    }

    public static BinacNumber operator +(BinacNumber first, BinacNumber second)
    {
        BinacNumber result;
        result.value = first.value + second.value;
        return Normalize(result);
    }

    public static BinacNumber operator -(BinacNumber first, BinacNumber second)
    {
        BinacNumber result;
        result.value = first.value - second.value;
        return Normalize(result);
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
        return new(a.value & 0x3F_FF_FF_FF);
    }

    private static int NormalizeCore(int a)
    {
        return a & 0x3F_FF_FF_FF;
    }
}
