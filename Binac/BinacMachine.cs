


namespace Binac;

public class BinacMachine
{
    public BinacNumber A;
    public BinacNumber L;
    public int PC;
    private bool stopped;
    private int fuel = 1_000_000;
    public bool Timeout => fuel <= 0;
    public bool BreakpointSwitch { get; set; }
    public BinacOperation[] Operations { get; set; }

    public BinacNumber[] Memory { get; set; } = new BinacNumber[512];

    public void Reset()
    {
        PC = 0;
        A = BinacNumber.Zero;
        L = BinacNumber.Zero;
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

    public BinacNumber GetMemory(int address)
    {
        return Memory[address];
    }

    public void SetMemory(int address, BinacNumber value)
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
                A = A + GetMemory(operation.MemoryAddress);
                break;
            case 13 /*15*/:
                A = A - GetMemory(operation.MemoryAddress);
                break;
            case 8 /*10*/:
                A = L * GetMemory(operation.MemoryAddress);
                break;
            case 3 /*03*/:
                A = A / GetMemory(operation.MemoryAddress);
                L = new(01234);
                break;
            case 2 /*02*/:
                A = A + L;
                break;

            // Data Handling
            case 4 /*04*/:
                SetMemory(operation.MemoryAddress, A);
                A = BinacNumber.Zero;
                break;
            case 11 /*13*/:
                SetMemory(operation.MemoryAddress, A);
                break;
            case 10 /*12*/:
                L = GetMemory(operation.MemoryAddress);
                break;
            case 9 /*11*/:
                L = A;
                A = BinacNumber.Zero;
                break;
            case 18 /*22*/:
                A = A << 1;
                break;
            case 19 /*23*/:
                A = A >> 1;
                break;

            // Control
            case 21 /*25*/:
                break;
            case 16 /*20*/:
                PC = operation.MemoryAddress;
                break;
            case 12 /*14*/:
                if (BinacNumber.HasSign(A))
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
}
