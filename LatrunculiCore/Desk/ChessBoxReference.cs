using System;

namespace LatrunculiCore.Desk
{
    public class ChessBoxReference
    {
        public readonly DeskSize DeskSize;

        public readonly int X;
        public readonly int Y;
        public int Number => Y + 1;
        public int Letter => X + 'A';

        public ChessBoxReference(DeskSize deskSize, int x, int y)
        {
            if (deskSize == null)
                throw new ArgumentNullException(nameof(deskSize));
            DeskSize = deskSize;
            if (x < 0 || x > (deskSize.X - 1))
                throw new ArgumentException($"X must be in range 0 to {deskSize.X - 1}.", nameof(x));
            if (y < 0 || y > (deskSize.Y - 1))
                throw new ArgumentException($"Y must be in range 0 to {deskSize.Y - 1}.", nameof(y));
            X = x;
            Y = y;
        }

        public static ChessBoxReference FromString(DeskSize deskSize, string str)
        {
            if (str.Length != 2)
                throw new ArgumentException($"Vstup musí mít délku 2.");
            str = str.ToUpper();
            char letter = str[0];
            char number = str[1];
            char lastLetter = (char)('A' + deskSize.X - 1);
            char lastNumber = (char)('0' + deskSize.Y - 1);
            if (letter < 'A' || letter > lastLetter)
                throw new ArgumentException($"Písmeno {letter} musí být mezi A a {lastLetter}.");
            if (number < '1' || number > lastNumber)
                throw new ArgumentException($"Číslo {number} musí být v rozmezí 1-{deskSize.X}.");
            return new ChessBoxReference(deskSize, (char)(letter - 'A'), (char)(number - '0' - 1));
        }
    }
}
