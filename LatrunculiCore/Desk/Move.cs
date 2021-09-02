using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LatrunculiCore.Desk
{
    public class Move : IEquatable<Move>
    {
        [JsonInclude]
        public ChessBoxPosition From { get; private set; }
        
        [JsonInclude]
        public ChessBoxPosition To { get; private set; }

        [JsonIgnore]
        public string HashCode => $"{From.Letter}{From.Number} {To.Letter}{To.Number}";

        public override string ToString() => HashCode;

        public bool Equals([AllowNull] Move other) => this == other;
        public static bool operator ==(Move a, Move b)
            => a?.HashCode == b?.HashCode;

        public static bool operator !=(Move a, Move b)
            => a?.HashCode != b?.HashCode;

        public override bool Equals(object obj) => obj is Move move && move == this;
        public override int GetHashCode() => HashCode.GetHashCode();

        [JsonConstructor]
        public Move(ChessBoxPosition from, ChessBoxPosition to)
        {
            From = from;
            To = to;
        }

        public Move CreateClone()
        {
            var originalDeskSize = From.DeskSize;
            var deskSize = new DeskSize(originalDeskSize.Width, originalDeskSize.Height);
            var from = new ChessBoxPosition(deskSize, From.X, From.Y);
            var to = new ChessBoxPosition(deskSize, To.X, To.Y);
            return new Move(from, to);
        }
    }
}
