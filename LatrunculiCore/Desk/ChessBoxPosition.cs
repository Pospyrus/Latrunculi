using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace LatrunculiCore.Desk
{
    [Flags]
    public enum NeighborDirection
    {
        None = 0,
        Vertical = 1,
        Horizontal = 2,
        All = Vertical | Horizontal
    }

    public class ChessBoxPosition : IEquatable<ChessBoxPosition>
    {
        [JsonInclude]
        public DeskSize DeskSize { get; private set; }
        
        [JsonIgnore]
        public string HashCode => $"{Letter}{Number}";

        public override string ToString() => HashCode;

        [JsonInclude]
        public int X { get; private set; }

        [JsonInclude]
        public int Y { get; private set; }
        
        [JsonIgnore]
        public int Number => Y + 1;
        
        [JsonIgnore]
        public char Letter => (char)(X + 'A');

        [JsonIgnore]
        public int Index => DeskSize.Height * X + Y;

        [JsonIgnore]
        public bool IsCorner => X == 0 && Y == 0 ||
                                X == DeskSize.Width - 1 && Y == 0 ||
                                X == 0 && Y == DeskSize.Height - 1 ||
                                X == DeskSize.Width - 1 && Y == DeskSize.Height - 1;

        [JsonConstructor]
        public ChessBoxPosition(DeskSize deskSize, int x, int y)
        {
            if (deskSize == null)
                throw new ArgumentNullException(nameof(deskSize));
            DeskSize = deskSize;
            if (x < 0 || x > (deskSize.Width - 1))
                throw new ArgumentException($"X must be in range 0 to {deskSize.Width - 1}.", nameof(x));
            if (y < 0 || y > (deskSize.Height - 1))
                throw new ArgumentException($"Y must be in range 0 to {deskSize.Height - 1}.", nameof(y));
            X = x;
            Y = y;
        }

        public bool Equals([AllowNull] ChessBoxPosition position) => position == this;
        public static bool operator ==(ChessBoxPosition a, ChessBoxPosition b)
            => a?.Index == b?.Index;

        public static bool operator !=(ChessBoxPosition a, ChessBoxPosition b)
            => a?.Index != b?.Index;

        public static bool IsValidPosition(DeskSize deskSize, int x, int y)
        {
            return x >= 0 && x < deskSize.Width && y >= 0 && y < deskSize.Height;
        }

        public static ChessBoxPosition TryGetPosition(DeskSize deskSize, int x, int y)
        {
            if (IsValidPosition(deskSize, x, y))
            {
                return new ChessBoxPosition(deskSize, x, y);
            }
            return null;
        }

        public static ChessBoxPosition FromString(DeskSize deskSize, string str)
        {
            if (str.Length != 2)
                throw new ArgumentException($"Vstup musí mít délku 2.");
            str = str.ToUpper();
            char letter = str[0];
            char number = str[1];
            char lastLetter = (char)('A' + deskSize.Width - 1);
            char lastNumber = (char)('0' + deskSize.Height);
            if (letter < 'A' || letter > lastLetter)
                throw new ArgumentException($"Písmeno {letter} musí být mezi A a {lastLetter}.");
            if (number < '1' || number > lastNumber)
                throw new ArgumentException($"Číslo {number} musí být v rozmezí 1-{deskSize.Height}.");
            return new ChessBoxPosition(deskSize, (char)(letter - 'A'), (char)(number - '0' - 1));
        }

        public IEnumerable<ChessBoxPosition> GetNeighbors(NeighborDirection direction = NeighborDirection.All)
        {
            var positions = new List<ChessBoxPosition>(4);
            if (direction.HasFlag(NeighborDirection.Horizontal))
            {
                positions.Add(TryGetPosition(DeskSize, X + 1, Y));
                positions.Add(TryGetPosition(DeskSize, X - 1, Y));
            }
            if (direction.HasFlag(NeighborDirection.Vertical))
            {
                positions.Add(TryGetPosition(DeskSize, X, Y + 1));
                positions.Add(TryGetPosition(DeskSize, X, Y - 1));
            }
            return from ChessBoxPosition newPosition in positions
                   where newPosition != null
                   select newPosition;
        }
    }
}
