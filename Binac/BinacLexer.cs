namespace Binac
{
    public class BinacLexer(string code)
    {
        public int Position { get; private set; }

        public bool TryParseOctalNumber(int position, out int value, out int nextPosition)
        {
            value = 0;
            nextPosition = position;
            if (!TryPeek(nextPosition, out var c))
            {
                nextPosition = position;
                return false;
            }

            if (!(c >= '0' && c <= '7'))
            {
                return false;
            }

            while (TryPeek(nextPosition, out c) && c >= '0' && c <= '7')
            {
                value = 8 * value + (c - '0');
                nextPosition++;
            }

            return true;
        }

        public bool TryPeek(out char c)
        {
            return TryPeek(Position, out c);
        }

        public bool TryPeek(int position, out char c)
        {
            if (position >= code.Length)
            {
                c = '\0';
                return false;
            }

            c = code[position];
            return true;
        }

        public void Consume(int nextPosition)
        {
            Position = nextPosition;
        }

        public bool TryParseCharacter(int position, char expectedCharacter, out int nextPosition)
        {
            nextPosition = position;
            if (TryPeek(position, out var c) && c == expectedCharacter)
            {
                nextPosition++;
                return true;
            }

            return false;
        }

        public bool TryConsumeCharacter(char expectedCharacter)
        {
            if (TryParseCharacter(Position, expectedCharacter, out var nextPosition))
            {
                Consume(nextPosition);
                return true;
            }

            return false;
        }

        public bool TryParseString(int position, string expectedString, out int nextPosition)
        {
            for (var i = 0; i < expectedString.Length; i++)
            {
                if (!TryPeek(position + i, out var c))
                {
                    nextPosition = position;
                    return false;
                }

                if (c != expectedString[i])
                {
                    nextPosition = position;
                    return false;
                }
            }

            nextPosition = position + expectedString.Length;
            return true;
        }

        public bool TryConsumeWhiteSpaces()
        {
            return TryConsumeWhiteSpaces(Position);
        }

        public bool TryConsumeWhiteSpaces(int position)
        {
            int nextPosition = position;
            while (TryPeek(nextPosition, out var c) && (c == ' ' || c == '\r' || c == '\t' || c == '\n'))
            {
                nextPosition++;
            }

            Consume(nextPosition);
            return true;
        }

        public bool TryConsumeString(string expectedString)
        {
            if (TryParseString(Position, expectedString, out var nextPosition))
            {
                Consume(nextPosition);
                return true;
            }

            return false;
        }
    }
}
