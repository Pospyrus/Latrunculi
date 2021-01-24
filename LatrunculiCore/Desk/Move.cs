using System;
using System.Diagnostics.CodeAnalysis;

namespace LatrunculiCore.Desk
{
    public class Move : IEquatable<Move>
    {
        public readonly ChessBoxPosition From;
        public readonly ChessBoxPosition To;

        public string HashCode => $"{(char)From.Letter}{From.Number} {(char)To.Letter}{To.Number}";

        public override string ToString() => HashCode;

        public bool Equals([AllowNull] Move other) => HashCode == other?.HashCode;

        public Move(ChessBoxPosition from, ChessBoxPosition to)
        {
            From = from;
            To = to;
        }
    }
}
