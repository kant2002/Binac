


namespace Binac;

public class BinacMachine
{
    public int A;
    public int L;
    public int PC;
    private bool stopped;
    private int fuel;
    public bool Timeout => fuel <= 0;
    public bool BreakpointSwitch { get; set; }
    public BinacOperation[] Operations { get; set; }

    public int[] Memory { get; set; } = new int[512];

    public void Reset()
    {
        PC = 0;
        A = 0;
        L = 0;
    }
    public void Run()
    {
        fuel = 1_000_000;
        stopped = false;
        while (!stopped && fuel > 0)
        {
            Step();
            fuel--;
        }
    }

    public void Step()
    {
        if (PC >= Operations.Length) return;

        var currentOp = Operations[PC];
        HandleOperation(currentOp);
        PC++;
    }

    public int GetMemory(int address)
    {
        return Memory[address];
    }

    public void SetMemory(int address, int value)
    {
        Memory[address] = value;
    }

    private void HandleOperation(BinacOperation operation)
    {
        // Handle the operation logic here
        // Example:
        switch (operation.Code)
        {
            // Arithmetic
            case 5:
                A += GetMemory(operation.MemoryAddress);
                A = Normalize(A);
                break;
            case 13 /*15*/:
                A -= GetMemory(operation.MemoryAddress);
                A = Normalize(A);
                break;
            case 8 /*10*/:
                A = Multiply(L, GetMemory(operation.MemoryAddress));
                A = Normalize(A);
                break;
            case 3 /*03*/:
                A = Divide(A, GetMemory(operation.MemoryAddress));
                A = Normalize(A);
                L = 01234;
                break;
            case 2 /*02*/:
                A += L;
                A = Normalize(A);
                break;

            // Data Handling
            case 4 /*04*/:
                SetMemory(operation.MemoryAddress, A);
                A = 0;
                break;
            case 11 /*13*/:
                SetMemory(operation.MemoryAddress, A);
                break;
            case 10 /*12*/:
                L = GetMemory(operation.MemoryAddress);
                break;
            case 9 /*11*/:
                L = A;
                A = 0;
                break;
            case 18 /*22*/:
                A = Normalize(A << 2);
                break;
            case 19 /*23*/:
                var sign = HasSign(A);
                A = Normalize(A >> 2) + (sign ? 0x40_00_00_00 : 0);
                break;

            // Control
            case 21 /*25*/:
                break;
            case 16 /*20*/:
                PC = operation.MemoryAddress;
                break;
            case 12 /*14*/:
                if (HasSign(A))
                {
                    PC = operation.MemoryAddress;
                }
                break;
            case 20 /*24*/:
                if (BreakpointSwitch)
                {
                    stopped = true;
                }
                break;
            case 1:
                stopped = true;
                break;
            // Add more cases as needed
        }
    }

    private int Multiply(int l, int v)
    {
        var sign = HasSign(l) ^ HasSign(v);
        return (Normalize(l) * Normalize(v) >> 30) + (sign ? 0x40_00_00_00 : 0);
    }

    private int Divide(int l, int v)
    {
        var sign = HasSign(l) ^ HasSign(v);
        return (int)((long)Normalize(l) << 30 / Normalize(v)) + (sign ? 0x40_00_00_00 : 0);
    }

    private bool HasSign(int a)
    {
        return (a & 0x40_00_00_00) != 0;
    }

    private int Normalize(int a)
    {
        return a & 0x3F_FF_FF_FF;
    }
}
