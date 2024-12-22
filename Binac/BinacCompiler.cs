namespace Binac;

public class BinacCompiler
{
    BinacLexer lexer;

    public BinacOperation[] ParseCode(string code)
    {
        List<BinacOperation> result = new();
        lexer = new BinacLexer(code);
        lexer.TryConsumeWhiteSpaces();
        while (TryParseCommand(out var operation))
        {
            lexer.TryConsumeWhiteSpaces();
            result.Add(operation);
        }

        lexer.TryConsumeWhiteSpaces();
        return result.ToArray();
    }

    public bool TryParseCommand(out BinacOperation operation)
    {
        operation = default;
        if (lexer.TryConsumeString("STOP"))
        {
            operation.Code = 1;
            return true;
        }

        if (lexer.TryConsumeString("SKIP"))
        {
            operation.Code = 25;
            return true;
        }

        if (lexer.TryConsumeString("BP"))
        {
            operation.Code = 24;
            return true;
        }

        if (TryParseNumberedCode(out operation))
        {
            return true;
        }

        if (TryParseLetteredCode(out operation))
        {
            return true;
        }

        return false;
    }


    public bool TryParseLetteredCode(out BinacOperation operation)
    {
        operation = default;
        int nextPosition;
        if (!lexer.TryPeek(out var code))
        {
            return false;
        }

        if (!new char[] { 'A', 'S' }.Contains(code))
        {
            return false;
        }

        if (!TryParseMemoryLocation(lexer.Position + 1, out var memoryLocation, out nextPosition))
        {
            return false;
        }

        operation.Code = code switch
        {
            'A' => 5,
            'S' => 13,  /* 15 */
            'M' => 8,   /* 10 */
            'D' => 3,
            'F' => 2,
            'C' => 4,
            'H' => 11,  /* 13 */
            'L' => 10,  /* 12 */
            'K' => 9,   /* 11 */
            '+' => 18,  /* 22 */
            '-' => 19,  /* 23 */
            'U' => 16,  /* 20 */
            'T' => 12,  /* 14 */
            _ => throw new NotSupportedException($"Unknown code {code}"),
        };
        operation.MemoryAddress = memoryLocation;
        lexer.Consume(nextPosition);
        return true;
    }
    public bool TryParseNumberedCode(out BinacOperation operation)
    {
        operation = default;
        int nextPosition;
        if (!lexer.TryParseOctalNumber(lexer.Position, out var code, out nextPosition))
        {
            return false;
        }

        if (!TryParseMemoryLocation(nextPosition, out var memoryLocation, out nextPosition))
        {
            return false;
        }

        operation.Code = (byte)code;
        operation.MemoryAddress = memoryLocation;
        lexer.Consume(nextPosition);
        return true;
    }

    public bool TryParseMemoryLocation(int position, out ushort operation, out int nextPosition)
    {
        operation = default;
        int cursor;
        nextPosition = position;
        if (!lexer.TryParseCharacter(position, '(', out cursor))
        {
            return false;
        }

        if (!lexer.TryParseOctalNumber(cursor, out var value, out cursor))
        {
            return false;
        }

        if (!lexer.TryParseCharacter(cursor, ')', out cursor))
        {
            return false;
        }

        nextPosition = cursor;
        operation = (ushort)value;
        return true;
    }
}
